using UnityEngine;

public class Tembak : MonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float bulletLifeTime = 5f;
    [SerializeField] private Transform firePoint;

    void Start()
    {
        // InvokeRepeating("Shoot", 0f, 0.2f);
    }

    public void Shoot()
    {
        if (bulletPrefab == null)
        {
            return;
        }

        Transform spawnPoint = firePoint != null ? firePoint : transform;

        GameObject bullet = Instantiate(bulletPrefab, spawnPoint.position, spawnPoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = spawnPoint.right * bulletSpeed;
        }

        Destroy(bullet, bulletLifeTime);
    }
}
