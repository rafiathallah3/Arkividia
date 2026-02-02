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
}
