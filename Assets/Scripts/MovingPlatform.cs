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

    [Header("Respawn Reset")]
    public bool resetOnPlayerRespawn = false;

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

        ResetPlatform();
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

    public void ResetPlatform()
    {
        if (pointA == null || pointB == null) return;

        transform.position = pointA.position;
        targetPoint = pointB;
        waitTimer = waitTime;
        lastPosition = transform.position;
        DeltaMovement = Vector3.zero;

        isActivated = !startOnPlayerEnter;
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