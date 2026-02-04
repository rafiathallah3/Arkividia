using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Hover Settings")]
    public float hoverScale = 1.05f;
    public Color hoverOutlineColor = Color.red;
    public float animationSpeed = 15f;

    [Header("Click Settings")]
    public float clickScale = 0.95f;
    public AudioClip clickSound;
    public UnityEvent OnClick;

    private Vector3 originalScale;
    private Outline outline;
    private Color originalOutlineColor;
    private AudioSource audioSource;

    private Vector3 targetScale;
    private Color targetOutlineColor;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;

        outline = GetComponent<Outline>();
        if (outline != null)
        {
            originalOutlineColor = outline.effectColor;
            targetOutlineColor = originalOutlineColor;
        }

        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
        if (outline != null)
        {
            outline.effectColor = Color.Lerp(outline.effectColor, targetOutlineColor, Time.deltaTime * animationSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * hoverScale;
        if (outline != null)
        {
            targetOutlineColor = hoverOutlineColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
        if (outline != null)
        {
            targetOutlineColor = originalOutlineColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        targetScale = originalScale * clickScale;
        OnClick?.Invoke();

        if (clickSound != null)
        {
            if (audioSource != null)
                audioSource.PlayOneShot(clickSound);
            else
                AudioSource.PlayClipAtPoint(clickSound, Camera.main.transform.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.hovered.Contains(gameObject))
        {
            targetScale = originalScale * hoverScale;
        }
        else
        {
            targetScale = originalScale;
        }
    }
}
