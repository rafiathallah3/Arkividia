using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TombolLevel : MonoBehaviour
{
    public float AwalAlpha = 130f;
    public float AkhirAlpha = 255f;

    [Header("Unpress Settings")]
    public bool bisaUnpress;
    public float delayUnpress;
    public UnityEvent OnUnpressed;

    private Vector3 originalScale;
    private SpriteRenderer spriteRenderer;
    public bool isPressed = false;

    [Header("Audio")]
    public AudioSource audioSource;

    void Start()
    {
        originalScale = transform.localScale;
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    public UnityEvent OnPressed;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPressed) return;

        if (collision.gameObject.TryGetComponent(out Pemain _))
        {
            OnPressed?.Invoke();
            if (audioSource != null)
            {
                audioSource.Play();
            }
            StartCoroutine(AnimateButton());
        }
    }

    private IEnumerator AnimateButton()
    {
        isPressed = true;

        Vector3 pressedScale = originalScale * 0.8f;
        float startAlpha = AwalAlpha / 255f;
        float endAlpha = AkhirAlpha / 255f;

        transform.localScale = pressedScale;
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = startAlpha;
            spriteRenderer.color = color;
        }

        yield return new WaitForSeconds(bisaUnpress ? delayUnpress : 0.1f);

        float duration = 0.3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localScale = Vector3.Lerp(pressedScale, originalScale, t);

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(startAlpha, endAlpha, t);
                spriteRenderer.color = color;
            }

            yield return null;
        }

        transform.localScale = originalScale;
        if (spriteRenderer != null)
        {
            Color color = spriteRenderer.color;
            color.a = endAlpha;
            spriteRenderer.color = color;
        }

        isPressed = false;

        if (bisaUnpress)
        {
            OnUnpressed?.Invoke();
        }
    }
}
