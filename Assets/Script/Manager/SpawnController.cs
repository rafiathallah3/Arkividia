using UnityEngine;
using System.Collections;

public class SpawnController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Settings")]
    [SerializeField] private float doorEntryDuration = 1.0f;
    [SerializeField] private float doorOpenWaitTime = 0.5f;
    [SerializeField] private float doorOpenDuration = 1.0f;

    [SerializeField] private Vector3 doorEntryOffset = new Vector3(0, -5, 0);
    [SerializeField] private float playerSpawnDuration = 1.0f;

    void Start()
    {
        if(spawnPoint == null)
        {
            spawnPoint = GameObject.Find("PosisiPintu").transform;
        }
    }

    public void StartLevelSequence()
    {
        StartCoroutine(CutsceneRoutine());
    }

    public event System.Action<Pemain> OnLevelSequenceFinished;

    private IEnumerator CutsceneRoutine()
    {
        Vector3 targetPos = spawnPoint.position;
        Vector3 startPos = targetPos + doorEntryOffset;

        GameObject doorInstance = Instantiate(doorPrefab, startPos, Quaternion.identity);

        float elapsed = 0f;
        while (elapsed < doorEntryDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / doorEntryDuration);
            float smoothT = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
            doorInstance.transform.position = Vector3.Lerp(startPos, targetPos, smoothT);
            yield return null;
        }
        doorInstance.transform.position = targetPos;

        AudioSource doorAudio = doorInstance.GetComponent<AudioSource>();
        if (doorAudio != null)
        {
            doorAudio.Play();
        }

        yield return new WaitForSeconds(doorOpenWaitTime);

        if (doorInstance.transform.childCount >= 2)
        {
            Transform pivot = doorInstance.transform.GetChild(1);
            Vector3 startScale = pivot.localScale;
            Vector3 targetScale = new Vector3(startScale.x, 0f, startScale.z);

            elapsed = 0f;
            while (elapsed < doorOpenDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / doorOpenDuration);
                float smoothT = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
                pivot.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
                yield return null;
            }
            pivot.localScale = targetScale;
        }
        GameObject playerInstance = Instantiate(playerPrefab, doorInstance.transform.GetChild(2).position, Quaternion.identity);
        Pemain pemainScript = playerInstance.GetComponent<Pemain>();

        if (pemainScript != null)
        {
            pemainScript.TriggerSpawnAnimation(playerSpawnDuration, false);
        }

        yield return new WaitForSeconds(playerSpawnDuration);

        if (pemainScript != null)
        {
            yield return new WaitUntil(() => pemainScript.IsGrounded);
        }

        Vector3 doorStartScale = doorInstance.transform.localScale;
        Vector3 doorTargetScale = new Vector3(doorStartScale.x, 0f, doorStartScale.z);

        float doorShrinkDuration = 1.0f;

        elapsed = 0f;
        while (elapsed < doorShrinkDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / doorShrinkDuration);
            float smoothT = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
            doorInstance.transform.localScale = Vector3.Lerp(doorStartScale, doorTargetScale, smoothT);
            yield return null;
        }
        doorInstance.transform.localScale = doorTargetScale;

        Destroy(doorInstance);

        OnLevelSequenceFinished?.Invoke(pemainScript);
    }
}
