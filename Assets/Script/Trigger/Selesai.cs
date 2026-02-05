using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Selesai : MonoBehaviour
{
    [SerializeField] private float suckDuration = 2.0f;
    [SerializeField] private float rotationSpeed = 360f;
    [SerializeField] private string kameraSelesaiName = "KameraSelesai";

    [Header("End Game Settings")]
    [SerializeField] private GameObject blackBackground;
    [SerializeField] private float fadeDuration = 5f;
    [SerializeField] private float waitBeforeDialogue = 2f;

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

        if (namaSceneLoad != "")
        {
            if(namaSceneLoad == "Selesai")
            {
                StartCoroutine(EndGameSequence());
            } else {
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
                SceneManager.LoadScene(namaSceneLoad);
            }
        }
    }
    private IEnumerator EndGameSequence()
    {
        if (blackBackground != null)
        {
            blackBackground.SetActive(true);
            Image img = blackBackground.GetComponent<Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;

                float elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.deltaTime;
                    c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                    img.color = c;
                    yield return null;
                }
                c.a = 1f;
                img.color = c;
            }
        }
        
        yield return new WaitForSeconds(waitBeforeDialogue);

        if (DialogueManager.instance != null)
        {
             DialogueManager.instance.ShowDialogue("Thank you for playing.", false, null, Color.cyan);
        }
    }
}
