using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : MonoBehaviour
{
    [Header("Laser Settings")]
    [SerializeField] private float maxLength = 10f;
    [SerializeField] private Color laserColor = Color.red;
    [SerializeField] private float startWidth = 0.1f;
    [SerializeField] private float endWidth = 0.1f;
    [SerializeField] private bool MatiSetelahHidup = false;
    [SerializeField] private float durasiHidup = 2f;

    [Header("Movement Settings")]
    [SerializeField] private Vector3 targetVector;
    [SerializeField] private bool loopMovement = true;
    [SerializeField] private float moveSpeed = 2f;

    private Vector3 initialPosition;
    private Vector3 destinationPosition;
    private bool movingToTarget = true;

    [Header("Collision Layers")]
    [SerializeField] private LayerMask obstacleLayer;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        lineRenderer.startWidth = startWidth;
        lineRenderer.endWidth = endWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        lineRenderer.startColor = laserColor;
        lineRenderer.endColor = laserColor;

        initialPosition = transform.position;
        destinationPosition = initialPosition + targetVector;
    }

    private void OnEnable()
    {
        if (lineRenderer != null) lineRenderer.enabled = true;
        if (MatiSetelahHidup)
        {
            CancelInvoke(nameof(MatikanLaser));
            Invoke(nameof(MatikanLaser), durasiHidup);
        }
    }

    private void OnDisable()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
        CancelInvoke(nameof(MatikanLaser));
    }

    private void MatikanLaser()
    {
        enabled = false;
    }

    private void Update()
    {
        MoveLaser();
        UpdateLaser();
    }

    private void MoveLaser()
    {
        if (moveSpeed <= 0) return;
        if (targetVector == Vector3.zero) return;

        Vector3 target = movingToTarget ? destinationPosition : initialPosition;
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            if (loopMovement)
            {
                movingToTarget = !movingToTarget;
            }
        }
    }

    private void UpdateLaser()
    {
        Vector3 startPos = transform.position;
        Vector3 direction = transform.up;

        lineRenderer.SetPosition(0, startPos);

        RaycastHit2D hit = Physics2D.Raycast(startPos, direction, maxLength, obstacleLayer);

        if (hit.collider != null)
        {
            lineRenderer.SetPosition(1, hit.point);

            Pemain pemain = hit.collider.GetComponent<Pemain>();
            if (pemain != null)
            {
                pemain.Die();
            }
        }
        else
        {
            Vector3 endPos = startPos + direction * maxLength;
            lineRenderer.SetPosition(1, endPos);
        }
    }

    private void OnValidate()
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = laserColor;
            lineRenderer.endColor = laserColor;
            lineRenderer.startWidth = startWidth;
            lineRenderer.endWidth = endWidth;
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = laserColor;
        Vector3 startPos = transform.position;
        Vector3 direction = transform.up;
        Gizmos.DrawLine(startPos, startPos + direction * maxLength);

        Gizmos.DrawWireSphere(startPos + direction * maxLength, 0.1f);

        if (!Application.isPlaying && targetVector != Vector3.zero)
        {
            Gizmos.color = Color.green;
            Vector3 pathStart = Application.isPlaying ? initialPosition : transform.position;
            Vector3 pathEnd = pathStart + targetVector;
            Gizmos.DrawLine(pathStart, pathEnd);
            Gizmos.DrawWireSphere(pathEnd, 0.2f);
        }
    }
}
