using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public GameObject dialoguePanel;
    public TMP_Text dialogueText;
    public float typingSpeed = 0.05f;
    public float slideDuration = 1.0f;
    public float autoHideDelay = 3.0f;

    private RectTransform panelRect;
    private Vector2 originalPosition;
    private Vector2 offScreenPosition;

    private Coroutine typingCoroutine;
    private Pemain player;
    private bool isPanelActive = false; // Tracks if panel is logically active/sliding in

    public static DialogueManager instance;

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

        player = FindAnyObjectByType<Pemain>();

        panelRect = dialoguePanel.GetComponent<RectTransform>();
        originalPosition = panelRect.anchoredPosition;
        offScreenPosition = originalPosition + new Vector2(0, Screen.height);
        panelRect.anchoredPosition = offScreenPosition;

        gameObject.SetActive(false);
        dialoguePanel.SetActive(false);
        // ShowDialogue("AAAAAAAAAAAAAAAAAAAAAAA Testing <slow>slow</slow> and <fast>fast</fast> speeds.");
    }

    public void ShowDialogue(string text, bool stopPlayer = false)
    {
        gameObject.SetActive(true);
        dialoguePanel.SetActive(true);

        player = FindAnyObjectByType<Pemain>();

        if (player != null)
        {
            if (stopPlayer)
            {
                player.isControllable = false;
                player.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
            else
            {
                player.isControllable = true;
            }
        }

        bool skipSlideIn = isPanelActive;
        isPanelActive = true;

        StopAllCoroutines();
        typingCoroutine = StartCoroutine(PlayDialogueSequence(text, skipSlideIn, stopPlayer));
    }

    public event System.Action OnDialogueFinished;

    IEnumerator PlayDialogueSequence(string text, bool skipSlideIn, bool stopPlayer)
    {
        dialogueText.text = "";

        if (!skipSlideIn)
        {
            yield return StartCoroutine(SlidePanel(offScreenPosition, originalPosition));
        }
        else
        {
            panelRect.anchoredPosition = originalPosition;
        }

        yield return StartCoroutine(TypeSentence(text));

        yield return new WaitForSeconds(autoHideDelay);

        yield return StartCoroutine(SlidePanel(originalPosition, offScreenPosition));

        isPanelActive = false;
        dialoguePanel.SetActive(false);
        gameObject.SetActive(false);

        if (stopPlayer && player != null)
        {
            player.isControllable = true;
        }

        OnDialogueFinished?.Invoke();
    }

    IEnumerator SlidePanel(Vector2 start, Vector2 end)
    {
        float elapsedTime = 0;
        while (elapsedTime < slideDuration)
        {
            float t = elapsedTime / slideDuration;
            float easedT = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;

            panelRect.anchoredPosition = Vector2.Lerp(start, end, easedT);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        panelRect.anchoredPosition = end;
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        bool isSlow = false;
        bool isFast = false;

        for (int i = 0; i < sentence.Length; i++)
        {
            if (sentence[i] == '<')
            {
                int endIndex = sentence.IndexOf('>', i);
                if (endIndex != -1)
                {
                    string tag = sentence.Substring(i, endIndex - i + 1);

                    if (tag == "<slow>")
                    {
                        isSlow = true;
                        i = endIndex;
                        continue;
                    }
                    else if (tag == "</slow>")
                    {
                        isSlow = false;
                        i = endIndex;
                        continue;
                    }
                    else if (tag == "<fast>")
                    {
                        isFast = true;
                        i = endIndex;
                        continue;
                    }
                    else if (tag == "</fast>")
                    {
                        isFast = false;
                        i = endIndex;
                        continue;
                    }
                    else
                    {
                        dialogueText.text += tag;
                        i = endIndex;
                        continue;
                    }
                }
            }

            dialogueText.text += sentence[i];

            float currentSpeed = typingSpeed;
            if (isSlow) currentSpeed *= 5.0f;
            if (isFast) currentSpeed *= 0.5f;

            yield return new WaitForSeconds(currentSpeed);
        }
    }
}
