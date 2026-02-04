using UnityEngine;

public class Tembak : MonoBehaviour
{
    [Header("Shooting Settings")]
    public bool shootAwake = false;
    public bool repeatShooting = false;
    public float shootingInterval = 1.5f;
    public bool useStartDelay = false;
    public bool randomizeStartDelay = false;
    [Range(0f, 10f)] public float startShootingDelay = 0f;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifeTime = 5f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Quaternion fireRotation;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (shootAwake)
        {
            float delay = GetInitialDelay();

            if (repeatShooting)
            {
                InvokeRepeating("Shoot", delay, shootingInterval);
            }
            else
            {
                if (delay > 0f)
                    Invoke("Shoot", delay);
                else
                    Shoot();
            }
        }
    }

    private float GetInitialDelay()
    {
        if (!useStartDelay)
            return 0f;

        if (randomizeStartDelay)
            return Random.Range(0f, startShootingDelay);

        return startShootingDelay;
    }

    public void Shoot()
    {
        if (bulletPrefab == null || gameObject.activeInHierarchy == false)
        {
            return;
        }

        Transform spawnPoint = firePoint != null ? firePoint : transform;
        audioSource.Play();

        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);
        bullet.transform.rotation = fireRotation;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if(rb == null)
        {
            rb = bullet.AddComponent<Rigidbody2D>();
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = spawnPoint.right * bulletSpeed;

        Destroy(bullet, bulletLifeTime);
    }
}
