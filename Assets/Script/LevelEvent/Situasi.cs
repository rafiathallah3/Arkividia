using UnityEngine;
using System.Collections;
using System.Security.Cryptography;

public class Situasi : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private GameObject doorPrefab;
    [SerializeField] private float doorEntryDuration = 1.0f;
    [SerializeField] private float doorOpenWaitTime = 0.5f;
    [SerializeField] private float doorOpenDuration = 1.0f;
    [SerializeField] private Vector3 doorEntryOffset = new Vector3(0, -5, 0);

    public void EnableTembak(Tembak penembak)
    {
        if (penembak != null)
        {
            penembak.enabled = true;
        }
    }

    public void DisableTembak(Tembak penembak)
    {
        if (penembak != null)
        {
            penembak.enabled = false;
        }
    }

    public void MulaiTembak(Tembak penembak)
    {
        if (penembak != null)
        {
            penembak.Shoot();
        }
    }

    public void HilanginPlatform(GameObject platform)
    {
        if (platform != null)
        {
            platform.SetActive(false);
        }
    }

    public void TunjuinGameObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(true);
        }
    }

    public void EnableLava(NaikLava lava)
    {
        if (lava != null)
        {
            lava.enabled = true;
        }
    }

    public void EnableGerakin(GerakinObject gerakin)
    {
        if (gerakin != null)
        {
            gerakin.enabled = true;
        }
    }

    public void EnableLaser(Laser laser)
    {
        if (laser != null)
        {
            laser.enabled = true;
        }
    }

    public void DisableLaser(Laser laser)
    {
        if (laser != null)
        {
            laser.enabled = false;
        }
    }

    public void HentikanShake()
    {
        if (KameraController.Instance != null)
        {
            KameraController.Instance.StopShake();
        }
    }

    public void MulaiAmbientSuara(AudioClip audioClip)
    {
        if (GameManager.instance != null && GameManager.instance.ambientAudioSource != null)
        {
            GameManager.instance.ambientAudioSource.Stop();
            GameManager.instance.ambientAudioSource.clip = audioClip;
            GameManager.instance.ambientAudioSource.Play();
        }
    }

    public void HentikanAMbientSuara()
    {
        if (GameManager.instance != null && GameManager.instance.ambientAudioSource != null)
        {
            GameManager.instance.ambientAudioSource.Stop();
        }
    }

    public void MainkanSuara(AudioSource audioSource)
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
    }

    public void MatikanPlayer()
    {
        Pemain player = FindAnyObjectByType<Pemain>();
        if (player != null)
        {
            player.Die();
        }
    }

    public void DisablePlayerMovement()
    {
        Pemain player = FindAnyObjectByType<Pemain>();
        if (player != null)
        {
            player.isControllable = false;
        }
    }

    public void EnablePlayerMovement()
    {
        Pemain player = FindAnyObjectByType<Pemain>();
        if (player != null)
        {
            player.isControllable = true;
        }
    }

    public void SimpanCutsceneSelesai()
    {
        GameManager.cutscenePlayed = true;
    }

    public void SimpanCutsceneSelesaiKedua()
    {
        GameManager.cutscenePlayed2 = true;
    }

    public void DashPemain()
    {
        Pemain player = FindAnyObjectByType<Pemain>();
        if (player != null)
        {
            player.TriggerDash();
        }
    }

    public void LaunchPemain(string arah)
    {
        Pemain player = FindAnyObjectByType<Pemain>();
        if (player != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                Vector2 direction = Vector2.right;

                switch (arah.ToLower())
                {
                    case "up":
                        direction = Vector2.up;
                        break;
                    case "down":
                        direction = Vector2.down;
                        break;
                    case "left":
                        direction = Vector2.left;
                        break;
                    case "right":
                        direction = Vector2.right;
                        break;
                }

                float kekuatan = 40f;
                rb.AddForce(direction * kekuatan, ForceMode2D.Impulse);
            }
        }
    }

    public void MunculinBossDiKedua()
    {
        Boss boss = FindAnyObjectByType<Boss>();
        if (boss != null)
        {
            boss.transform.position = new Vector3(-35.42f, 9.35f, 0f);
        }
    }

    public void MunculinBossDiKetiga()
    {
        Boss boss = FindAnyObjectByType<Boss>();
        if (boss != null)
        {
            boss.transform.position = new Vector3(-7.29f, 36.58f, 0f);
        }
    }

    public void MunculkanPintu(Transform lokasi)
    {
        if (doorPrefab != null && lokasi != null)
        {
            StartCoroutine(ProsesMunculkanPintu(lokasi.position));
        }
    }

    private IEnumerator ProsesMunculkanPintu(Vector3 targetPos)
    {
        Vector3 startPos = targetPos + doorEntryOffset;
        GameObject doorInstance = Instantiate(doorPrefab, startPos, Quaternion.identity);

        float elapsed = 0f;
        while (elapsed < doorEntryDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / doorEntryDuration);
            float smoothT = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
            doorInstance.transform.position = Vector3.Lerp(startPos, targetPos, smoothT);
            yield return null;
        }
        doorInstance.transform.position = targetPos;

        AudioSource doorAudio = doorInstance.GetComponent<AudioSource>();
        if (doorAudio != null)
        {
            doorAudio.Play();
        }

        yield return new WaitForSeconds(doorOpenWaitTime);

        if (doorInstance.transform.childCount >= 2)
        {
            Transform pivot = doorInstance.transform.GetChild(1);
            Vector3 startScale = pivot.localScale;
            Vector3 targetScale = new Vector3(startScale.x, 0f, startScale.z);

            elapsed = 0f;
            while (elapsed < doorOpenDuration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / doorOpenDuration);
                float smoothT = -(Mathf.Cos(Mathf.PI * t) - 1) / 2;
                pivot.localScale = Vector3.Lerp(startScale, targetScale, smoothT);
                yield return null;
            }
            pivot.localScale = targetScale;
        }
    }
}
