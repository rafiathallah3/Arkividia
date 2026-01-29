using UnityEngine;
using System.Collections;

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
    public bool IsGrounded => isGrounded;
    private float horizontalInput;

    // Control State
    public bool isControllable = true;

    public GameObject deathParticleEffect;

    Transform sprite;

    // Dash State
    private bool isDashing;
    private bool canDash = true;
    private float facingDirection = 1f;
    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        sprite = transform.Find("Sprite").transform;
    }

    private void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        }
        else
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, groundLayer);
        }

        if (!isControllable) return;
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

        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (!isControllable) return;
        if (isDashing) return;

        float targetSpeed = horizontalInput * kecepatan;
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

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;

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

    public void Die()
    {
        isControllable = false;
        rb.linearVelocity = Vector2.zero;

        if (deathParticleEffect != null)
        {
            Instantiate(deathParticleEffect, transform.position, Quaternion.identity);
        }

        sprite.gameObject.SetActive(false);
        StartCoroutine(TungguMati(2f));
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

    public void TriggerSpawnAnimation(float duration, bool autoEnableControl = true)
    {
        StartCoroutine(SpawnSequence(duration, autoEnableControl));
    }

    private IEnumerator TungguMati(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.SetActive(false);
        GameManager.instance.StartLevelSequence();
        Destroy(gameObject);
    }

    private IEnumerator SpawnSequence(float duration, bool autoEnableControl)
    {
        isControllable = false;
        rb.simulated = false;
        transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);

            float rotation = Mathf.Lerp(0f, 360f, t);
            transform.rotation = Quaternion.Euler(0, 0, rotation);

            yield return null;
        }

        transform.localScale = originalScale;
        transform.rotation = Quaternion.identity;

        rb.simulated = true;
        if (autoEnableControl)
        {
            isControllable = true;
        }
    }

    public void EnterFinishState()
    {
        isControllable = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
    }
}
