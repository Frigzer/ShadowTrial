using System.Collections;
using UnityEngine;

public class PushingWall : MonoBehaviour
{
    [Header("Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float startDelay = 0f;

    [Header("Behavior")]
    public bool activateOnlyOnce = false;
    public bool returnToStart = false;
    public float returnDelay = 1.5f;
    public bool resetOnPlayerRespawn = true;

    private bool isActivated = false;
    private bool isMoving = false;
    private Coroutine moveRoutine;

    private void Start()
    {
        if (startPoint == null || endPoint == null)
        {
            Debug.LogError("PushingWall: Missing startPoint or endPoint.", this);
            enabled = false;
            return;
        }

        transform.position = startPoint.position;
    }

    public void ActivateWall()
    {
        if (isMoving) return;
        if (activateOnlyOnce && isActivated) return;

        isActivated = true;
        moveRoutine = StartCoroutine(MoveSequence());
    }

    private IEnumerator MoveSequence()
    {
        isMoving = true;

        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        yield return StartCoroutine(MoveToPoint(endPoint.position));

        if (returnToStart)
        {
            if (returnDelay > 0f)
            {
                yield return new WaitForSeconds(returnDelay);
            }

            yield return StartCoroutine(MoveToPoint(startPoint.position));
        }

        isMoving = false;
        moveRoutine = null;
    }

    private IEnumerator MoveToPoint(Vector3 targetPosition)
    {
        while (Vector3.Distance(transform.position, targetPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = targetPosition;
    }

    public void ResetWall()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }

        transform.position = startPoint.position;
        isMoving = false;
        isActivated = false;
    }

    private void OnDrawGizmos()
    {
        if (startPoint == null || endPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPoint.position, endPoint.position);
        Gizmos.DrawSphere(startPoint.position, 0.2f);
        Gizmos.DrawSphere(endPoint.position, 0.2f);
    }
}