using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Boss : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private float flashDuration = 0.5f;

    [Header("Dialogue Settings")]
    [TextArea]
    [SerializeField] private System.Collections.Generic.List<string> phaseDialogues;
    [SerializeField] private System.Collections.Generic.List<EventSituasi> phaseEvents;
    public int currentHitCount = 0;

    [Header("Death Settings")]
    [SerializeField] private GameObject whiteBackgroundReference;
    [SerializeField] private GameObject deathParticlePrefab;
    [SerializeField] private float shakeMagnitude = 0.5f;
    [SerializeField] private float waitBeforeFade = 2f;
    [SerializeField] private float waitWhileWhite = 2f;
    [SerializeField] private float waitBeforeDialogue = 2f;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    private GameObject spriteObject;
    private Color originalColor;
    private bool isTakingDamage = false;
    private GameObject particleContainer;
    private new Light2D light;


    void Start()
    {
        light = GetComponent<Light2D>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        spriteObject = transform.GetChild(0).gameObject;
        spriteRenderer = spriteObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
        if (whiteBackgroundReference != null)
        {
            var img = whiteBackgroundReference.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                Color c = img.color;
                c.a = 0f;
                img.color = c;
            }
            whiteBackgroundReference.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Pemain pemain = collision.gameObject.GetComponent<Pemain>();
            if (pemain != null && pemain.IsDashing && !isTakingDamage)
            {
                audioSource.Play();
                Vector2 knockbackDirection = (transform.position - collision.transform.position).normalized;
                StartCoroutine(TakeDamage(knockbackDirection));
            }
        }
    }

    private System.Collections.IEnumerator TakeDamage(Vector2 direction)
    {
        isTakingDamage = true;
        
        bool isDeathHit = currentHitCount == 2; // 3rd hit (index 2)

        if (phaseDialogues != null && currentHitCount < phaseDialogues.Count)
        {
            int hitIndex = currentHitCount;
            if (DialogueManager.instance != null)
            {
                DialogueManager.instance.ShowDialogue(phaseDialogues[currentHitCount], true, () => 
                {
                    if (isDeathHit)
                    {
                         StartCoroutine(DeathSequenceAfterDialogue());
                    }
                    else
                    {
                        // Normal event logic
                        if (phaseEvents != null && hitIndex < phaseEvents.Count)
                        {
                             EventSituasi evt = phaseEvents[hitIndex];
                             if (evt != null)
                             {
                                 if (evt.delay > 0)
                                 {
                                     StartCoroutine(ExecuteEventWithDelay(evt));
                                 }
                                 else
                                 {
                                     evt.onEventTriggered?.Invoke();
                                 }
                             }
                        }
                    }
                });
            }

            if (isDeathHit)
            {
                StartDeathEffects();
            }

            currentHitCount++;
        }
        else if (isDeathHit) // Fallback if no dialogue for hit 3
        {
             currentHitCount++;
             StartDeathEffects();
             StartCoroutine(DeathSequenceAfterDialogue());
             yield break;
        }

        if (rb != null)
        {
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
            StartCoroutine(StopKnockbackAfterDelay(knockbackDuration));
        }

        float elapsed = 0f;
        while (elapsed < flashDuration)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
                yield return new WaitForSeconds(0.1f);
                spriteRenderer.color = originalColor;
                yield return new WaitForSeconds(0.1f);
            }
            elapsed += 0.2f;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        isTakingDamage = false;
    }

    private void StartDeathEffects()
    {
        if (KameraController.Instance != null)
        {
            KameraController.Instance.StartShake(shakeMagnitude);
        }

        if (deathParticlePrefab != null)
        {
            particleContainer = Instantiate(deathParticlePrefab, transform.position, Quaternion.identity, gameObject.transform);
        }
    }

    private System.Collections.IEnumerator DeathSequenceAfterDialogue()
    {
        // Wait before fade (camera still shaking)
        yield return new WaitForSeconds(waitBeforeFade);

        if (whiteBackgroundReference != null)
        {
            whiteBackgroundReference.SetActive(true);
            UnityEngine.UI.Image img = whiteBackgroundReference.GetComponent<UnityEngine.UI.Image>();
            if (img != null)
            {
                float duration = 2f;
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    elapsed += Time.deltaTime;
                    Color c = img.color;
                    c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
                    img.color = c;
                    yield return null;
                }
                Color final = img.color;
                final.a = 1f;
                img.color = final;
            }
            else
            {
                yield return new WaitForSeconds(2f);
            }
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }

        // Wait while totally white
        yield return new WaitForSeconds(waitWhileWhite);

        light.enabled = false;
        if (spriteObject != null) spriteObject.SetActive(false);
        if (rb != null) rb.simulated = false;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
        if(particleContainer != null) particleContainer.SetActive(false);
        
        if (KameraController.Instance != null)
        {
            KameraController.Instance.StopShake();
        }

        // Disappear immediately
        if (whiteBackgroundReference != null)
        {
            whiteBackgroundReference.SetActive(false);
        }

        yield return new WaitForSeconds(waitBeforeDialogue);

        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.ShowDialogue("I'll write my own story.", true, () => 
            {
                 Destroy(gameObject);
            }, Color.cyan);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator ExecuteEventWithDelay(EventSituasi evt)
    {
        yield return new WaitForSeconds(evt.delay);
        evt.onEventTriggered?.Invoke();
    }

    private System.Collections.IEnumerator StopKnockbackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
}
