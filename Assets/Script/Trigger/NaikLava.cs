using UnityEngine;

public class NaikLava : MonoBehaviour
{
    public Vector3 targetSize;
    public float duration = 1.0f;

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetSize, Time.deltaTime / duration);
        if (KameraController.Instance != null)
        {
            KameraController.Instance.StartShake(0.1f);
        }
    }
}
