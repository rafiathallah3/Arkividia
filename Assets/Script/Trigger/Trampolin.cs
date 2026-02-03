using UnityEngine;

public class Trampolin : MonoBehaviour
{
    public float Kekuatan = 10f;
    public Vector2 directionOffset;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Pemain _))
        {
            if (audioSource != null)
            {
                audioSource.Play();
            }
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                Vector2 direction = ((Vector2)transform.up + directionOffset).normalized;
                rb.AddForce(direction * Kekuatan, ForceMode2D.Impulse);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector2 direction = ((Vector2)transform.up + directionOffset).normalized;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)direction * Kekuatan);
        Gizmos.DrawSphere(transform.position + (Vector3)direction * Kekuatan, 0.1f);
    }
}
