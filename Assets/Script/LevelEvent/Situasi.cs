using UnityEngine;

public class Situasi : MonoBehaviour
{
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
}
