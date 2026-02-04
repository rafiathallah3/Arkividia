using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Selesai : MonoBehaviour
{
    [SerializeField] private float suckDuration = 2.0f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private string kameraSelesaiName = "KameraSelesai";

    public string namaSceneLoad;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Pemain player = other.GetComponent<Pemain>();
            if (player != null)
            {
                audioSource.Play();
                StartCoroutine(SuckInPlayer(player));
            }
        }
    }

    private IEnumerator SuckInPlayer(Pemain player)
    {
        player.EnterFinishState();

        Vector3 startPosition = player.transform.position;
        Vector3 endPosition = transform.position;
        Vector3 startScale = player.transform.localScale;

        float elapsed = 0f;

        while (elapsed < suckDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / suckDuration;
            float tSmooth = t * t * (3f - 2f * t);

            player.transform.position = Vector3.Lerp(startPosition, endPosition, tSmooth);
            player.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, tSmooth);

            player.transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            yield return null;
        }

        player.transform.position = endPosition;
        player.transform.localScale = Vector3.zero;

        GameObject kameraSelesai = GameObject.Find(kameraSelesaiName);
        if (kameraSelesai != null)
        {
            KameraController.Instance.SetOverrideTarget(kameraSelesai.transform);

            while (!KameraController.Instance.IsAtTarget)
            {
                yield return null;
            }

            yield return new WaitForSeconds(2.5f);
        }
        else
        {
            Debug.LogWarning($"Object '{kameraSelesaiName}' not found!");
        }

        if (namaSceneLoad != "")
        {
            SceneManager.LoadScene(namaSceneLoad);
        }
    }
}
