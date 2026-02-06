using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool ApakahDebug = false;

    private DialogueManager dialogueManager;
    private SpawnController spawnController;
    private LevelConfig levelConfig;
    public GameObject CanvasUtamaUI;
    public AudioSource sfxAudioSource;
    public AudioSource ambientAudioSource;
    public AudioSource OSTAudioSource;

    public static bool cutscenePlayed = false;
    public static bool cutscenePlayed2 = false;
    private Boss BossInstance;

    public GameObject TunjuinPlatformSetelah2;

    public static GameManager instance;
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if(CanvasUtamaUI != null)
        {
            CanvasUtamaUI.SetActive(true);
        }

        dialogueManager = FindFirstObjectByType<DialogueManager>();
        spawnController = FindFirstObjectByType<SpawnController>();
        levelConfig = FindFirstObjectByType<LevelConfig>();
        BossInstance = FindFirstObjectByType<Boss>();

        if(OSTAudioSource != null)
        {
            OSTAudioSource.Play();
        }

        if(SceneManager.GetActiveScene().name == "Level 4")
        {
            KamarKamera kamarKameraAwal = GameObject.Find("Kamar1").transform.GetChild(0).GetComponent<KamarKamera>();
            if(cutscenePlayed)
            {
                BossInstance.currentHitCount++;
                GameObject.Find("TempatCutsceen1").SetActive(false);
                kamarKameraAwal.followMaxPosition = new Vector3(34f, 0f, 0f);
                BossInstance.transform.position = new Vector3(19.75f, 7f, 0f);
            }

            if(cutscenePlayed2)
            {
                BossInstance.currentHitCount++;
                kamarKameraAwal.transform.position = new Vector3(-24.8f, 10.85f, -10f);
                kamarKameraAwal.cameraMode = CameraMode.Fixed;
                kamarKameraAwal.cameraCenter = kamarKameraAwal.transform;
                GameObject.Find("PosisiPintu").transform.position = new Vector3(-24.88f, 6.16f, 0);
                BossInstance.transform.position = new Vector3(-35.42f, 9.224488f, 0f);
                Camera.main.transform.position = new Vector3(-24.8f, 10.85f, -10f);
                // KameraController.Instance.SetOverrideTarget(kamarKameraAwal.transform);
                TunjuinPlatformSetelah2.SetActive(true);
                // KameraController.Instance.ClearOverrideTarget();
            }
        }

        if (spawnController != null)
        {
            StartLevelSequence();
        }
    }

    public void StartLevelSequence()
    {
        KameraController.Instance.ResetCamera();
        spawnController.OnLevelSequenceFinished += OnSpawnFinished;
        spawnController.StartLevelSequence();
    }

    private void OnSpawnFinished(Pemain player)
    {
        spawnController.OnLevelSequenceFinished -= OnSpawnFinished;

        if (dialogueManager != null && levelConfig.hasIntroDialogue)
        {
            System.Action onDialogueFinished = null;
            onDialogueFinished = () =>
            {
                dialogueManager.OnDialogueFinished -= onDialogueFinished;
                if (player != null)
                {
                    player.isControllable = true;
                }

                foreach (EventSituasi eventSituasi in levelConfig.introEvents)
                {
                    StartCoroutine(ExecuteIntroEvent(eventSituasi));
                }
            };

            dialogueManager.OnDialogueFinished += onDialogueFinished;
            dialogueManager.ShowDialogue(levelConfig.introDialogueText);

            if (ApakahDebug)
            {
                player.isControllable = true;
            }
        }
        else if (player != null)
        {
            player.isControllable = true;
        }

        if(!levelConfig.hasIntroDialogue)
        {
            foreach (EventSituasi eventSituasi in levelConfig.introEvents)
            {
                StartCoroutine(ExecuteIntroEvent(eventSituasi));
            }
        }
    }

    IEnumerator ExecuteIntroEvent(EventSituasi eventSituasi)
    {
        yield return new WaitForSeconds(eventSituasi.delay);
        eventSituasi.onEventTriggered?.Invoke();
    }
}
