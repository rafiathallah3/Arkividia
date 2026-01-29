using UnityEngine;

public class KameraController : MonoBehaviour
{
    public static KameraController Instance;

    public Transform player;
    public float smoothTime = 0.2f;

    Vector3 velocity;
    KamarKamera currentRoom;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if(GameObject.FindGameObjectWithTag("Player") != null && player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void LateUpdate()
    {
        if (currentRoom == null) return;

        if (currentRoom.cameraMode == CameraMode.Fixed)
        {
            MoveTo(currentRoom.cameraCenter.position);
        }
        else if (currentRoom.cameraMode == CameraMode.Follow)
        {
            FollowPlayer();
        }
    }

    public void ResetCamera()
    {
        currentRoom = GameObject.Find("Kamar1").transform.GetChild(0).GetComponent<KamarKamera>();
    }

    public void EnterRoom(KamarKamera room)
    {
        currentRoom = room;
    }

    void MoveTo(Vector3 target)
    {
        Vector3 pos = Vector3.SmoothDamp(
            transform.position,
            new Vector3(target.x, target.y, transform.position.z),
            ref velocity,
            smoothTime
        );
        transform.position = pos;
    }

    void FollowPlayer()
    {
        if(player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null) return;
        }

        Vector3 target = player.position;

        target.x = Mathf.Clamp(
            target.x,
            currentRoom.followMinPosition.x,
            currentRoom.followMaxPosition.x
        );

        target.y = Mathf.Clamp(
            target.y,
            currentRoom.followMinPosition.y,
            currentRoom.followMaxPosition.y
        );

        MoveTo(target);
    }
}
