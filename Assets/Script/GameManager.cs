using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool ApakahDebug = false;

    private DialogueManager dialogueManager;
    private SpawnController spawnController;

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

        dialogueManager = FindFirstObjectByType<DialogueManager>();
        spawnController = FindFirstObjectByType<SpawnController>();

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

        if (dialogueManager != null)
        {
            System.Action onDialogueFinished = null;
            onDialogueFinished = () =>
            {
                dialogueManager.OnDialogueFinished -= onDialogueFinished;
                if (player != null)
                {
                    player.isControllable = true;
                }
                StartCoroutine(DelayedShootingSequence());
            };

            dialogueManager.OnDialogueFinished += onDialogueFinished;
            dialogueManager.ShowDialogue("You will <slow>die</slow> in <slow>2 seconds</slow> from <fast>upcoming bullets</fast>");
            if(ApakahDebug)
            {
                player.isControllable = true;
            }
        }
        else if (player != null)
        {
            player.isControllable = true;
            StartCoroutine(DelayedShootingSequence());
        }
    }

    private System.Collections.IEnumerator DelayedShootingSequence()
    {
        yield return new WaitForSeconds(2f);

        Tembak[] shooters = FindObjectsByType<Tembak>(FindObjectsSortMode.None);
        foreach (Tembak shooter in shooters)
        {
            if (shooter != null)
            {
                shooter.Shoot();
            }
        }
    }
}
