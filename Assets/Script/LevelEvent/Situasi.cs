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
        if(platform != null)
        {
            Destroy(platform);
        }
    }

    public void TunjuinGameObject(GameObject obj)
    {
        if(obj != null)
        {
            obj.SetActive(true);
        }
    }
}
