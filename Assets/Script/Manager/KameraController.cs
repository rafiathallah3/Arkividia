using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class KameraController : MonoBehaviour
{
    public static KameraController Instance;

    public Pemain player;
    public float smoothTime = 0.2f;

    [Header("Background Settings")]
    public RectTransform backgroundTransform;
    public float backgroundSpeed = 0.5f;
    public Vector2 backgroundAmplitude = new Vector2(60f, 60f);
    public Color darkColor = new Color(0.78431374f, 0.78431374f, 0.78431374f, 1f);
    public float fadeDuration = 1.0f;

    [Header("Vignette Settings")]
    public RectTransform vignetteTransform;
    public Color vignetteDarkColor = new Color(0, 0, 0, 0.8f);

    private Vector2 backgroundStartPos;
    private Image backgroundImage;
    private Color originalColor;
    private Coroutine fadeCoroutine;

    private Image vignetteImage;
    private Color vignetteOriginalColor;
    private Coroutine vignetteFadeCoroutine;

    Vector3 velocity;
    KamarKamera currentRoom;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null && player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Pemain>();
        }

        if (backgroundTransform != null)
        {
            backgroundStartPos = backgroundTransform.anchoredPosition;
            backgroundImage = backgroundTransform.GetComponent<Image>();
            if (backgroundImage != null)
            {
                originalColor = backgroundImage.color;
            }
        }

        if (vignetteTransform == null)
        {
            vignetteTransform = GameObject.Find("Vignette")?.GetComponent<RectTransform>();
        }

        if (vignetteTransform != null)
        {
            vignetteImage = vignetteTransform.GetComponent<Image>();
            if (vignetteImage != null)
            {
                vignetteOriginalColor = new Color(1f, 1f, 1f, 0.62352943f);
                vignetteImage.color = vignetteOriginalColor;
            }
        }
    }

    void Update()
    {
        if (backgroundTransform != null)
        {
            float x = Mathf.Sin(Time.time * backgroundSpeed) * backgroundAmplitude.x;
            float y = Mathf.Cos(Time.time * backgroundSpeed * 0.9f) * backgroundAmplitude.y;
            backgroundTransform.anchoredPosition = backgroundStartPos + new Vector2(x, y);
        }
    }

    public void DarkenBackground()
    {
        if (backgroundImage != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeRoutine(backgroundImage, darkColor, fadeDuration));
        }

        if (vignetteImage != null)
        {
            if (vignetteFadeCoroutine != null) StopCoroutine(vignetteFadeCoroutine);
            vignetteFadeCoroutine = StartCoroutine(FadeRoutine(vignetteImage, vignetteDarkColor, fadeDuration));
        }
    }

    public void RestoreBackground()
    {
        if (backgroundImage != null)
        {
            if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
            fadeCoroutine = StartCoroutine(FadeRoutine(backgroundImage, originalColor, fadeDuration));
        }

        if (vignetteImage != null)
        {
            if (vignetteFadeCoroutine != null) StopCoroutine(vignetteFadeCoroutine);
            vignetteFadeCoroutine = StartCoroutine(FadeRoutine(vignetteImage, vignetteOriginalColor, fadeDuration));
        }
    }

    IEnumerator FadeRoutine(Image image, Color targetColor, float duration)
    {
        Color startColor = image.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            image.color = Color.Lerp(startColor, targetColor, elapsed / duration);
            yield return null;
        }

        image.color = targetColor;
    }

    private bool isShaking = false;
    private float shakeMagnitude = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    public void StartShake(float magnitude)
    {
        if (isShaking) return;
        isShaking = true;
        shakeMagnitude = magnitude;
    }

    public void StopShake()
    {
        isShaking = false;
        shakeMagnitude = 0f;
        shakeOffset = Vector3.zero;
    }

    private Transform overrideTarget;

    public void SetOverrideTarget(Transform target)
    {
        overrideTarget = target;
    }

    public void ClearOverrideTarget()
    {
        overrideTarget = null;
    }

    void LateUpdate()
    {
        if (overrideTarget != null)
        {
            MoveTo(overrideTarget.position);
            return;
        }

        if (isShaking)
        {
            shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            shakeOffset.z = 0;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        if (currentRoom == null) return;

        if (currentRoom.cameraMode == CameraMode.Fixed)
        {
            MoveTo(currentRoom.cameraCenter.position + shakeOffset);
        }
        else if (currentRoom.cameraMode == CameraMode.Follow)
        {
            FollowPlayer();
        }
    }

    public void ResetCamera()
    {
        EnterRoom(GameObject.Find("Kamar1").transform.GetChild(0).GetComponent<KamarKamera>());
        if (currentRoom.cameraMode == CameraMode.Fixed)
        {
            _activeTarget = currentRoom.cameraCenter.position;
        }
        else
        {
            _activeTarget = currentRoom.followMinPosition;
        }
    }

    public void EnterRoom(KamarKamera room)
    {
        currentRoom = room;
    }

    private Vector3 _activeTarget;
    public bool IsAtTarget => Vector3.Distance(transform.position, _activeTarget) < 0.01f;

    void MoveTo(Vector3 target)
    {
        _activeTarget = new Vector3(target.x, target.y, transform.position.z);
        Vector3 pos = Vector3.SmoothDamp(
            transform.position,
            _activeTarget,
            ref velocity,
            smoothTime
        );
        transform.position = pos;
    }

    void FollowPlayer()
    {
        if (player == null)
        {
            if (GameObject.FindGameObjectWithTag("Player") == null)
            {
                MoveTo(currentRoom.followMinPosition);
                return;
            }
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Pemain>();
        }
        
        if (player.SudahMati)
        {
            MoveTo(currentRoom.followMinPosition);
            return;
        }

        Vector3 target = player.transform.position;

        target.x = Mathf.Clamp(
            target.x,
            currentRoom.followMinPosition.x,
            currentRoom.followMaxPosition.x
        );

        target.y = Mathf.Clamp(
            target.y,
            currentRoom.followMinPosition.y,
            currentRoom.followMaxPosition.y
        );

        MoveTo(target + shakeOffset);
    }
}
