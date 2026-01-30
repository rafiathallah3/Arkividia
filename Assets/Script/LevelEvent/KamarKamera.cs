using UnityEngine;

public enum CameraMode
{
    Fixed,
    Follow
}
public class KamarKamera : MonoBehaviour
{
    [Header("Camera Mode")]
    public CameraMode cameraMode;

    [Header("Fixed Mode Settings")]
    public Transform cameraCenter;
    
    [Header("Follow Mode Settings")]
    public Vector3 followMinPosition;
    public Vector3 followMaxPosition;

    public EdgeCollider2D cameraBounds;

    void Start()
    {
        cameraCenter = transform.parent;
        cameraBounds = GetComponent<EdgeCollider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            KameraController.Instance.EnterRoom(this);
        }
    }
}
