using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Pemain : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float kecepatan = 5f;
    [SerializeField] private float akselerasi = 10f;
    [SerializeField] private float deselerasi = 10f;
    [SerializeField] private float lompat = 5f;

    [SerializeField] private float tiltAmount = 10f;
    [SerializeField] private float tiltSpeed = 10f;

    [Header("Dash Settings")]
    [SerializeField] private float kecepatanDash = 20f;
    [SerializeField] private float durasiDash = 0.2f;
    [SerializeField] private float cooldownDash = 1f;
    [SerializeField] private Vector3 dashShrinkScale = new Vector3(0.8f, 0.8f, 1f);

    [Header("Gravity Settings")]
    [SerializeField] private float scaleGravitasi = 1f;
    [SerializeField] private float jatuhMultiplier = 2.5f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isGrounded;
    private float horizontalInput;

    // Dash State
    private bool isDashing;
    private bool canDash = true;
    private float facingDirection = 1f;
    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (isDashing) return;

        horizontalInput = 0f;
        if (Input.GetKey(KeyCode.A))
        {
            horizontalInput = -1f;
            facingDirection = -1f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            horizontalInput = 1f;
            facingDirection = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canDash)
        {
            StartCoroutine(DashCoroutine());
        }

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        }

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        float targetSpeed = horizontalInput * kecepatan;
        float speedDif = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? akselerasi : deselerasi;
        float newSpeed = Mathf.MoveTowards(rb.linearVelocity.x, targetSpeed, accelRate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(newSpeed, rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = scaleGravitasi * jatuhMultiplier;
        }
        else
        {
            rb.gravityScale = scaleGravitasi;
        }

        // Tilt Logic
        float targetTilt = 0f;
        if (!isGrounded)
        {
            targetTilt = -rb.linearVelocity.x * 2f;
            targetTilt = Mathf.Clamp(targetTilt, -tiltAmount, tiltAmount);
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetTilt);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime);
    }

    private System.Collections.IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(facingDirection * kecepatanDash, 0f);

        transform.localScale = dashShrinkScale;

        yield return new WaitForSeconds(durasiDash);

        rb.gravityScale = scaleGravitasi;
        transform.localScale = originalScale;
        isDashing = false;

        yield return new WaitForSeconds(cooldownDash);
        canDash = true;
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, lompat);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
