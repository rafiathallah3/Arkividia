using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform mainCamera;
    public float moveSpeed = 1.0f;
    public float moveAmount = 0.5f;

    [Header("Background Settings")]
    public Image background;
    public Color darkColor = Color.gray;
    public Color semiBrightColor = Color.white;
    public float pulseSpeed = 1.0f;

    [Header("Title Settings")]
    public TMP_Text titleText;
    public float glitchIntervalMin = 2.0f;
    public float glitchIntervalMax = 5.0f;
    public float titleDeletionSpeed = 0.1f;

    [Header("Start Sequence Settings")]
    public RectTransform[] menuButtons; // For start sequence animation
    public AudioClip startSound;
    public float buttonExitSpeed = 500f; // Pixels per second
    public Color brightFlashColor = Color.white;
    public float flashDuration = 0.5f;
    public GerakinObject[] gerakinObjects;

    [Header("Option Menu Settings")]
    public RectTransform optionFrame;
    public float optionAnimationDuration = 1.0f;

    private Vector3 startPos;
    private string originalText;
    private bool isStarting = false;
    private bool isOptionsOpen = false;

    private Vector2 optionFrameClosedPos;
    private Vector2 optionFrameOpenPos;
    private Dictionary<RectTransform, Vector2> buttonsOriginalPos = new Dictionary<RectTransform, Vector2>();

    void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main.transform;

        if (mainCamera != null)
            startPos = mainCamera.position;

        if (titleText != null)
        {
            originalText = titleText.text;
            StartCoroutine(GlitchRoutine());
        }

        foreach (RectTransform btn in menuButtons)
        {
            if (btn != null) buttonsOriginalPos[btn] = btn.anchoredPosition;
        }

        if (optionFrame != null)
        {
            optionFrameOpenPos = optionFrame.anchoredPosition;
            optionFrameClosedPos = optionFrameOpenPos + new Vector2(0, -Screen.height);
            optionFrame.anchoredPosition = optionFrameClosedPos;
        }
    }

    void Update()
    {
        if (!isStarting)
        {
            HandleCameraMovement();
            HandleBackgroundEffect();
        }
    }

    void HandleCameraMovement()
    {
        if (mainCamera == null) return;

        float x = Mathf.Sin(Time.time * moveSpeed) * moveAmount;
        float y = Mathf.Cos(Time.time * moveSpeed) * moveAmount;

        mainCamera.position = startPos + new Vector3(x, y, 0);
    }

    void HandleBackgroundEffect()
    {
        if (background == null) return;

        float t = Mathf.PingPong(Time.time * pulseSpeed, 1.0f);
        background.color = Color.Lerp(darkColor, semiBrightColor, t);
    }

    public void OpenOptionSequence()
    {
        if (isStarting || isOptionsOpen) return;
        StartCoroutine(AnimateOptions(true));
    }

    public void CloseOptionSequence()
    {
        if (isStarting || !isOptionsOpen) return;
        StartCoroutine(AnimateOptions(false));
    }

    IEnumerator AnimateOptions(bool open)
    {
        isOptionsOpen = open;
        float timer = 0f;

        while (timer < optionAnimationDuration)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / optionAnimationDuration);
            float easedT = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;

            foreach (var kvp in buttonsOriginalPos)
            {
                RectTransform btn = kvp.Key;
                Vector2 original = kvp.Value;
                Vector2 hidden = original + new Vector2(0, -Screen.height * 1.2f);
                Vector2 start = open ? original : hidden;
                Vector2 end = open ? hidden : original;

                btn.anchoredPosition = Vector2.Lerp(start, end, easedT);
            }

            if (optionFrame != null)
            {
                Vector2 start = open ? optionFrameClosedPos : optionFrameOpenPos;
                Vector2 end = open ? optionFrameOpenPos : optionFrameClosedPos;

                optionFrame.anchoredPosition = Vector2.Lerp(start, end, easedT);
            }

            yield return null;
        }

        foreach (var kvp in buttonsOriginalPos)
        {
            RectTransform btn = kvp.Key;
            Vector2 original = kvp.Value;
            Vector2 hidden = original + new Vector2(0, -Screen.height * 1.2f);
            btn.anchoredPosition = open ? hidden : original;
        }

        if (optionFrame != null)
        {
            optionFrame.anchoredPosition = open ? optionFrameOpenPos : optionFrameClosedPos;
        }
    }

    public void StartGameSequence()
    {
        if (isStarting) return;
        isStarting = true;

        if (startSound != null)
            AudioSource.PlayClipAtPoint(startSound, mainCamera.position);

        StartCoroutine(StartSequenceCoroutine());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    IEnumerator StartSequenceCoroutine()
    {
        float timer = 0f;

        foreach (GerakinObject gerakin in gerakinObjects)
        {
            gerakin.enabled = true;
        }

        Dictionary<RectTransform, Vector2> buttonStartPos = new Dictionary<RectTransform, Vector2>();
        float buttonDuration = 1.5f;
        foreach (RectTransform btn in menuButtons)
        {
            if (btn != null) buttonStartPos[btn] = btn.anchoredPosition;
        }

        float deletionTimer = 0f;
        int totalChars = titleText.textInfo.characterCount;
        titleText.maxVisibleCharacters = totalChars;
        int currentVisible = totalChars;

        Color initialBgColor = background != null ? background.color : Color.white;

        while (true)
        {
            timer += Time.deltaTime;

            bool buttonsDone = timer >= buttonDuration;
            bool textDone = currentVisible <= 0;
            bool cameraDone = false;

            if (!buttonsDone)
            {
                float t = Mathf.Clamp01(timer / buttonDuration);
                float easedT = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;

                foreach (var kvp in buttonStartPos)
                {
                    RectTransform btn = kvp.Key;
                    Vector2 start = kvp.Value;
                    Vector2 target = start + new Vector2(0, -Screen.height * 1.2f);
                    btn.anchoredPosition = Vector2.Lerp(start, target, easedT);
                }
            }
            else
            {
                foreach (var kvp in buttonStartPos)
                {
                    Vector2 start = kvp.Value;
                    Vector2 target = start + new Vector2(0, -Screen.height * 1.2f);
                    kvp.Key.anchoredPosition = target;
                }
            }

            if (!textDone)
            {
                deletionTimer += Time.deltaTime;
                if (deletionTimer >= titleDeletionSpeed)
                {
                    deletionTimer = 0f;
                    currentVisible--;
                    titleText.maxVisibleCharacters = currentVisible;
                }
            }

            if (background != null)
            {
                background.color = Color.Lerp(initialBgColor, brightFlashColor, Mathf.Clamp01(timer / flashDuration));
            }

            if (mainCamera != null)
            {
                mainCamera.position = Vector3.Lerp(mainCamera.position, new Vector3(0, 0, -10), Time.deltaTime * 2.0f);
                if (Vector3.Distance(mainCamera.position, new Vector3(0, 0, -10)) < 0.01f)
                {
                    mainCamera.position = new Vector3(0, 0, -10);
                    cameraDone = true;
                }
            }
            else
            {
                cameraDone = true;
            }

            if (buttonsDone && textDone && cameraDone)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("Level Tutorial");
    }

    IEnumerator GlitchRoutine()
    {
        while (!isStarting)
        {
            yield return new WaitForSeconds(Random.Range(glitchIntervalMin, glitchIntervalMax));
            if (!isStarting)
                yield return StartCoroutine(PerformGlitch());
        }
    }

    IEnumerator PerformGlitch()
    {
        int glitchCount = Random.Range(1, 4);
        for (int i = 0; i < glitchCount; i++)
        {
            if (isStarting) break;
            titleText.text = GenerateGlitchedText();
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
            titleText.text = originalText;
            yield return new WaitForSeconds(Random.Range(0.05f, 0.15f));
        }
        titleText.text = originalText;
    }

    string GenerateGlitchedText()
    {
        if (string.IsNullOrEmpty(originalText)) return "";

        char[] chars = originalText.ToCharArray();
        List<int> validIndices = new List<int>();
        bool inTag = false;

        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == '<')
            {
                inTag = true;
                continue;
            }
            if (chars[i] == '>')
            {
                inTag = false;
                continue;
            }

            if (!inTag && !char.IsWhiteSpace(chars[i]))
            {
                validIndices.Add(i);
            }
        }

        if (validIndices.Count > 0)
        {
            int charsToGlitch = Random.Range(1, 3);
            for (int k = 0; k < charsToGlitch; k++)
            {
                int rndIdx = validIndices[Random.Range(0, validIndices.Count)];
                chars[rndIdx] = '_';
            }
        }

        return new string(chars);
    }
}
