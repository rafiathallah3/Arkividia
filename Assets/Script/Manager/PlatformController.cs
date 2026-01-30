using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [Header("References")]
    public SpriteRenderer outlineRenderer;
    public BoxCollider2D platformCollider;

    void Awake()
    {
        if (platformCollider == null)
        {
            platformCollider = GetComponent<BoxCollider2D>();
        }

        if (outlineRenderer == null)
        {
            outlineRenderer = GetComponent<SpriteRenderer>();
        }

        if (platformCollider != null)
        {
            platformCollider.size = new Vector2(
                outlineRenderer.size.x,
                outlineRenderer.size.y
            );
            
            platformCollider.offset = Vector2.zero;
        }
    }

    void OnValidate()
    {
        if (platformCollider != null)
        {
            platformCollider.size = new Vector2(
                outlineRenderer.size.x,
                outlineRenderer.size.y
            );
            
            platformCollider.offset = Vector2.zero;
        }
    }
}