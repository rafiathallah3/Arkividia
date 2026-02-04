using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AktifSpoiler : MonoBehaviour
{
    [Header("State")]
    public bool apakahSudahAktif = false;
    public bool bisaDiUlanginEvent = false;
    public bool TunjuinDialogLagi = false;

    [Header("Settingan")]
    public string textSpoiler;
    public AudioSource audioSource;
    public List<EventSituasi> events;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && (!apakahSudahAktif || bisaDiUlanginEvent))
        {
            bool didShowDialogue = false;
            if (textSpoiler != "" && (!apakahSudahAktif || TunjuinDialogLagi))
            {
                DialogueManager.instance.ShowDialogue(textSpoiler, true);
                didShowDialogue = true;
            }
            apakahSudahAktif = true;

            if (didShowDialogue && DialogueManager.instance != null)
            {
                DialogueManager.instance.OnDialogueFinished += StartEventsAfterDialogue;
            }
            else
            {
                foreach (EventSituasi eventSituasi in events)
                {
                    StartCoroutine(ExecuteIntroEvent(eventSituasi));
                }
            }
        }
    }

    private void StartEventsAfterDialogue()
    {
        if (DialogueManager.instance != null)
            DialogueManager.instance.OnDialogueFinished -= StartEventsAfterDialogue;

        foreach (EventSituasi eventSituasi in events)
        {
            StartCoroutine(ExecuteIntroEvent(eventSituasi));
        }
    }

    IEnumerator ExecuteIntroEvent(EventSituasi eventSituasi)
    {
        yield return new WaitForSeconds(eventSituasi.delay);
        eventSituasi.onEventTriggered?.Invoke();
    }
}
