using System.Collections;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [Header("Timing")]
    public float delayBeforeFall = 0.5f;
    public float respawnDelay = 2f;

    [Header("Behavior")]
    public bool respawn = true;
    public bool resetOnPlayerRespawn = true;

    [Header("Shake")]
    public bool shakeBeforeFall = true;
    public float shakeDuration = 0.25f;
    public float shakeStrength = 0.05f;
    public Transform visualTarget;

    [Header("References")]
    public Collider[] platformColliders;
    public Renderer[] platformRenderers;

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Vector3 visualStartLocalPosition;

    private bool isTriggered = false;
    private Coroutine fallRoutine;

    private void Awake()
    {
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (platformColliders == null || platformColliders.Length == 0)
        {
            platformColliders = GetComponentsInChildren<Collider>(true);
        }

        if (platformRenderers == null || platformRenderers.Length == 0)
        {
            platformRenderers = GetComponentsInChildren<Renderer>(true);
        }

        if (visualTarget == null)
        {
            visualTarget = transform;
        }

        visualStartLocalPosition = visualTarget.localPosition;
    }

    public void TriggerPlatform()
    {
        if (isTriggered) return;
        fallRoutine = StartCoroutine(FallSequence());
    }

    private IEnumerator FallSequence()
    {
        isTriggered = true;

        float waitBeforeShake = delayBeforeFall - shakeDuration;

        if (waitBeforeShake > 0f)
        {
            yield return new WaitForSeconds(waitBeforeShake);
        }

        if (shakeBeforeFall)
        {
            yield return StartCoroutine(ShakeRoutine());
        }

        SetPlatformActive(false);

        if (respawn)
        {
            yield return new WaitForSeconds(respawnDelay);
            ResetPlatform();
        }
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;

            Vector3 randomOffset = new Vector3(
                Random.Range(-shakeStrength, shakeStrength),
                Random.Range(-shakeStrength, shakeStrength),
                Random.Range(-shakeStrength, shakeStrength)
            );

            visualTarget.localPosition = visualStartLocalPosition + randomOffset;

            yield return null;
        }

        visualTarget.localPosition = visualStartLocalPosition;
    }

    private void SetPlatformActive(bool active)
    {
        foreach (Collider col in platformColliders)
        {
            if (col != null)
            {
                col.enabled = active;
            }
        }

        foreach (Renderer rend in platformRenderers)
        {
            if (rend != null)
            {
                rend.enabled = active;
            }
        }
    }

    public void ResetPlatform()
    {
        if (fallRoutine != null)
        {
            StopCoroutine(fallRoutine);
            fallRoutine = null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (visualTarget != null)
        {
            visualTarget.localPosition = visualStartLocalPosition;
        }

        SetPlatformActive(true);
        isTriggered = false;
    }
}