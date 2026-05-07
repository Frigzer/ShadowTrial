using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement")]
    public float speed = 2f;
    public float waitTime = 0.5f;

    [Header("Activation")]
    public bool startOnPlayerEnter = false;
    public bool activateOnlyOnce = true;

    public Vector3 DeltaMovement { get; private set; }

    private Transform targetPoint;
    private float waitTimer;
    private Vector3 lastPosition;

    private bool isActivated = false;

    private void Start()
    {
        if (pointA == null || pointB == null)
        {
            Debug.LogError("MovingPlatform: PointA or PointB is missing.", this);
            enabled = false;
            return;
        }

        transform.position = pointA.position;
        targetPoint = pointB;
        waitTimer = waitTime;
        lastPosition = transform.position;

        if (!startOnPlayerEnter)
        {
            isActivated = true;
        }
    }

    private void Update()
    {
        if (!isActivated)
        {
            DeltaMovement = Vector3.zero;
            lastPosition = transform.position;
            return;
        }

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
        }
        else
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPoint.position,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
            {
                targetPoint = targetPoint == pointA ? pointB : pointA;
                waitTimer = waitTime;
            }
        }

        DeltaMovement = transform.position - lastPosition;
        lastPosition = transform.position;
    }

    public void ActivatePlatform()
    {
        isActivated = true;
    }

    private void OnControllerColliderHitProxy(Transform other)
    {
        if (!startOnPlayerEnter) return;
        if (!other.CompareTag("Player")) return;

        ActivatePlatform();
    }

    private void OnDrawGizmos()
    {
        if (pointA == null || pointB == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA.position, pointB.position);
        Gizmos.DrawSphere(pointA.position, 0.15f);
        Gizmos.DrawSphere(pointB.position, 0.15f);
    }
}