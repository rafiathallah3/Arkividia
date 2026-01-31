using UnityEngine;

public class GerakinObject : MonoBehaviour
{
    public enum MoveDirection { UP, DOWN, LEFT, RIGHT, CUSTOM }

    public MoveDirection direction;
    public float distance = 5f;
    public Vector3 customTargetPosition;
    public float speed = 2f;

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float startTime;
    private float journeyLength;

    void Start()
    {
        startPosition = transform.position;

        switch (direction)
        {
            case MoveDirection.UP:
                targetPosition = startPosition + Vector3.up * distance;
                break;
            case MoveDirection.DOWN:
                targetPosition = startPosition + Vector3.down * distance;
                break;
            case MoveDirection.LEFT:
                targetPosition = startPosition + Vector3.left * distance;
                break;
            case MoveDirection.RIGHT:
                targetPosition = startPosition + Vector3.right * distance;
                break;
            case MoveDirection.CUSTOM:
                targetPosition = customTargetPosition;
                break;
        }

        journeyLength = Vector3.Distance(startPosition, targetPosition);
        startTime = Time.time;
    }

    void Update()
    {
        if (journeyLength > 0)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
        }
    }
}
