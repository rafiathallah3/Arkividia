using System.Collections;
using UnityEngine;

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

        if (spawnController != null)
        {
            StartLevelSequence();
        }

        if(OSTAudioSource != null)
        {
            OSTAudioSource.Play();
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
