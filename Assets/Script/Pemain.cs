using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class Pemain : MonoBehaviour
{
    public bool SudahMati = false;
    bool SudahMatiSementara = false;

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
    private GameObject DashEffectObject;

    [Header("Gravity Settings")]
    [SerializeField] private float scaleGravitasi = 1f;
    [SerializeField] private float jatuhMultiplier = 2.5f;

    [Header("Ground Detection")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask safeCrushLayer;

    [Header("Suara")]
    public AudioSource audioSource;
    public AudioClip Lompat;
    public AudioClip Mati;
    public AudioClip SuaraDash;
    public AudioClip SuaraMendarat;

    private Rigidbody2D rb;
    private Collider2D col;
    private new Light2D light;
    private bool isGrounded;
    public bool IsGrounded => isGrounded;
    private bool wasGrounded;
    private float horizontalInput;

    // Control State
    public bool isControllable = true;

    public GameObject landingParticleEffect;
    public GameObject deathParticleEffect;
    public GameObject tambahanDeathParticle;

    private float speedMultiplier = 1f;

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    Transform sprite;
    TrailRenderer trailRenderer;
    TrailRenderer[] trails;

    private bool isDashing;
    public bool IsDashing => isDashing;
    private bool canDash = true;
    private float facingDirection = 1f;
    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
        sprite = transform.Find("Sprite").transform;
        col = GetComponent<Collider2D>();
        trailRenderer = GetComponent<TrailRenderer>();
        light = GetComponent<Light2D>();
        DashEffectObject = transform.Find("DashEffect").gameObject;
        trails = DashEffectObject.GetComponentsInChildren<TrailRenderer>();

        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        SetTrailAlpha(0f);
    }

    // Lombat: Jumped off the bench
    // Landed: Sound: Landed on the floor with his feet after jumping
    // Dash: Strong mace strike

    public void Die()
    {
        if (SudahMati || SudahMatiSementara) return;
        SudahMatiSementara = true;
        isControllable = false;
        rb.linearVelocity = Vector2.zero;
        rb.simulated = false;
        col.enabled = false;

        if (audioSource != null && Mati != null)
        {
            audioSource.PlayOneShot(Mati);
        }

        if (deathParticleEffect != null)
        {
            tambahanDeathParticle = Instantiate(deathParticleEffect, transform.position, Quaternion.identity);
        }

        KameraController.Instance.StopShake();
        GameManager.instance.ambientAudioSource.Stop();
        sprite.gameObject.SetActive(false);
        trailRenderer.enabled = false;
        light.enabled = false;
        StartCoroutine(TungguMati(2f));
    }

    private void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, lompat);
        if (audioSource != null && Lompat != null)
        {
            audioSource.PlayOneShot(Lompat);
        }
    }

    private void Update()
    {
        LayerMask effectiveGround = groundLayer | safeCrushLayer;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, effectiveGround);
        }
        else
        {
            isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1.1f, effectiveGround);
        }

        if (!wasGrounded && isGrounded)
        {
            if (audioSource != null && SuaraMendarat != null)
            {
                audioSource.PlayOneShot(SuaraMendarat);
            }

            if (landingParticleEffect != null)
            {
                GameObject particle = Instantiate(landingParticleEffect, groundCheck.position, Quaternion.identity);
                Destroy(particle, 1f);
            }

            if (!isDashing && Mathf.Abs(rb.linearVelocity.x) > kecepatan)
            {
                rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -kecepatan, kecepatan), rb.linearVelocity.y);
            }
        }

        wasGrounded = isGrounded;

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
            TriggerDash();
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

        float targetSpeed = horizontalInput * kecepatan * speedMultiplier;
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

        float targetTilt = 0f;
        if (!isGrounded)
        {
            targetTilt = -rb.linearVelocity.x * 2f;
            targetTilt = Mathf.Clamp(targetTilt, -tiltAmount, tiltAmount);
        }

        Quaternion targetRotation = Quaternion.Euler(0, 0, targetTilt);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, tiltSpeed * Time.fixedDeltaTime);

        if(SceneManager.GetActiveScene().name == "Level 3") { //Jangan tanya kenapa harus cek hanya di level 3, pokoknya CheckCrush() ini agak bemasalah
            CheckForCrush();
        }
    }

    private void CheckForCrush()
    {
        if (!isControllable) return;

        if (col != null)
        {
            float checkDistance = 0.02f;
            Bounds bounds = col.bounds;

            RaycastHit2D hitTop = Physics2D.Raycast(bounds.center, Vector2.up, bounds.extents.y + checkDistance, groundLayer);
            RaycastHit2D hitBottom = Physics2D.Raycast(bounds.center, Vector2.down, bounds.extents.y + checkDistance, groundLayer);

            RaycastHit2D hitLeft = Physics2D.Raycast(bounds.center, Vector2.left, bounds.extents.x + checkDistance, groundLayer);
            RaycastHit2D hitRight = Physics2D.Raycast(bounds.center, Vector2.right, bounds.extents.x + checkDistance, groundLayer);

            if (hitTop && hitBottom)
            {
                if (!IsSafe(hitTop) && !IsSafe(hitBottom))
                {
                    Die();
                }
            }
            else if (hitLeft && hitRight)
            {
                if (!IsSafe(hitLeft) && !IsSafe(hitRight))
                {
                    Die();
                }
            }
        }
    }

    private bool IsSafe(RaycastHit2D hit)
    {
        if (hit.collider == null) return false;
        return ((1 << hit.collider.gameObject.layer) & safeCrushLayer) != 0;
    }

    public void TriggerDash()
    {
        StartCoroutine(DashCoroutine());
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true;
        DashEffectObject.SetActive(true);
        SetTrailAlpha(1f);

        if (audioSource != null && SuaraDash != null)
        {
            audioSource.PlayOneShot(SuaraDash);
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = new Vector2(facingDirection * kecepatanDash, 0f);
        transform.localScale = dashShrinkScale;

        yield return new WaitForSeconds(durasiDash);

        rb.gravityScale = scaleGravitasi;
        transform.localScale = originalScale;
        isDashing = false;

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(Mathf.Clamp(rb.linearVelocity.x, -kecepatan, kecepatan), rb.linearVelocity.y);
        }

        StartCoroutine(FadeTrail());

        yield return new WaitForSeconds(cooldownDash);
        canDash = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        Gizmos.color = Color.yellow;
        if (col != null)
        {
            Bounds b = col.bounds;
            float dist = 0.1f;
            Gizmos.DrawLine(b.center, b.center + Vector3.up * (b.extents.y + dist));
            Gizmos.DrawLine(b.center, b.center + Vector3.down * (b.extents.y + dist));
            Gizmos.DrawLine(b.center, b.center + Vector3.left * (b.extents.x + dist));
            Gizmos.DrawLine(b.center, b.center + Vector3.right * (b.extents.x + dist));
        }
    }

    private void SetTrailAlpha(float alpha)
    {
        if (DashEffectObject == null) return;

        foreach (TrailRenderer trail in trails)
        {
            if (trail == null) continue;

            Color start = trail.startColor;
            start.a = alpha;
            trail.startColor = start;

            Color end = trail.endColor;
            end.a = alpha;
            trail.endColor = end;
        }
    }

    private IEnumerator FadeTrail()
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            SetTrailAlpha(alpha);
            yield return null;
        }

        SetTrailAlpha(0f);
        DashEffectObject.SetActive(false);
    }

    public void TriggerSpawnAnimation(float duration, bool autoEnableControl = true)
    {
        StartCoroutine(SpawnSequence(duration, autoEnableControl));
    }

    private IEnumerator TungguMati(float delay)
    {
        yield return new WaitForSeconds(delay);

        SudahMati = true;
        KameraController.Instance.ResetCamera();

        float timeout = 5.0f;
        float elapsed = 0f;
        while (!KameraController.Instance.IsAtTarget && elapsed < timeout)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
