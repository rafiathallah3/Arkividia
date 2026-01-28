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

        panelRect = dialoguePanel.GetComponent<RectTransform>();
        originalPosition = panelRect.anchoredPosition;
        offScreenPosition = originalPosition - new Vector2(0, Screen.height);
        panelRect.anchoredPosition = offScreenPosition;

        gameObject.SetActive(false);
        dialoguePanel.SetActive(false);
        instance.ShowDialogue("AAAAAAAAAAAAAAAAAAAAAAA Testing <slow>slow</slow> and <fast>fast</fast> speeds.");
    }

    public void ShowDialogue(string text)
    {
        gameObject.SetActive(true);
        dialoguePanel.SetActive(true);
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(PlayDialogueSequence(text));
    }

    IEnumerator PlayDialogueSequence(string text)
    {
        // Slide In
        yield return StartCoroutine(SlidePanel(offScreenPosition, originalPosition));

        // Type Text
        yield return StartCoroutine(TypeSentence(text));

        // Wait
        yield return new WaitForSeconds(autoHideDelay);

        // Slide Out
        yield return StartCoroutine(SlidePanel(originalPosition, offScreenPosition));

        dialoguePanel.SetActive(false);
        gameObject.SetActive(false);
    }

    IEnumerator SlidePanel(Vector2 start, Vector2 end)
    {
        float elapsedTime = 0;
        while (elapsedTime < slideDuration)
        {
            float t = elapsedTime / slideDuration;
            // Ease In Out Sine
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
                // Find closing bracket
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
            if (isSlow) currentSpeed *= 5.0f; // Slower means higher delay
            if (isFast) currentSpeed *= 0.5f; // Faster means lower delay

            yield return new WaitForSeconds(currentSpeed);
        }
    }
}
