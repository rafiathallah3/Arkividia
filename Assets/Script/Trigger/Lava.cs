using UnityEngine;

public class Lava : MonoBehaviour
{
    [SerializeField] private float slowDownFactor = 0.5f;
    private Collider2D lavaCollider;

    private void Awake()
    {
        lavaCollider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Pemain pemain = other.GetComponent<Pemain>();
        if (pemain != null)
        {
            pemain.SetSpeedMultiplier(slowDownFactor);
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Pemain pemain = other.GetComponent<Pemain>();
        if (pemain != null && !pemain.SudahMati)
        {
            if (IsCompletelyInside(other.bounds, lavaCollider.bounds))
            {
                pemain.Die();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Pemain pemain = other.GetComponent<Pemain>();
        if (pemain != null)
        {
            pemain.SetSpeedMultiplier(1f);
        }
    }

    private bool IsCompletelyInside(Bounds inner, Bounds outer)
    {
        return inner.min.x >= outer.min.x && inner.max.x <= outer.max.x &&
               inner.min.y >= outer.min.y && inner.max.y <= outer.max.y;
    }
}
