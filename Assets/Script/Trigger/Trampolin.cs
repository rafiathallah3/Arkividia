using UnityEngine;

public class Trampolin : MonoBehaviour
{
    public float Kekuatan = 10f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Pemain _))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.AddForce(transform.up * Kekuatan, ForceMode2D.Impulse);
            }
        }
    }
}
