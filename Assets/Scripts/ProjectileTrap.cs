using System.Collections;
using UnityEngine;

public class ProjectileTrap : MonoBehaviour
{
    [Header("References")]
    public TrapProjectile projectilePrefab;
    public Transform[] spawnPoints;
    public Transform targetPoint;

    [Header("Activation")]
    public bool activeOnStart = true;

    [Header("Burst Settings")]
    public int shotsPerBurst = 3;
    public float timeBetweenShots = 0.2f;
    public float timeBetweenBursts = 2f;
    public float startDelay = 0f;

    [Header("Spawn Mode")]
    public bool fireAllSpawnPointsAtOnce = true;
    public bool alternateSpawnPoints = false;

    private bool isActive = false;
    private Coroutine fireRoutine;
    private int currentSpawnIndex = 0;

    private void Start()
    {
        if (projectilePrefab == null || spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("ProjectileTrap: Missing projectilePrefab or spawnPoints.", this);
            enabled = false;
            return;
        }

        if (activeOnStart)
        {
            ActivateTrap();
        }
    }

    public void ActivateTrap()
    {
        if (isActive) return;

        isActive = true;

        if (fireRoutine == null)
        {
            fireRoutine = StartCoroutine(FireLoop());
        }
    }

    public void DeactivateTrap()
    {
        if (!isActive) return;

        isActive = false;

        if (fireRoutine != null)
        {
            StopCoroutine(fireRoutine);
            fireRoutine = null;
        }
    }

    private IEnumerator FireLoop()
    {
        if (startDelay > 0f)
        {
            yield return new WaitForSeconds(startDelay);
        }

        while (isActive)
        {
            yield return StartCoroutine(FireBurst());

            if (isActive)
            {
                yield return new WaitForSeconds(timeBetweenBursts);
            }
        }

        fireRoutine = null;
    }

    private IEnumerator FireBurst()
    {
        for (int i = 0; i < shotsPerBurst; i++)
        {
            if (!isActive) yield break;

            FireProjectiles();

            if (i < shotsPerBurst - 1)
            {
                yield return new WaitForSeconds(timeBetweenShots);
            }
        }
    }

    private void FireProjectiles()
    {
        if (fireAllSpawnPointsAtOnce)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                FireFromPoint(spawnPoints[i]);
            }

            return;
        }

        if (alternateSpawnPoints)
        {
            FireFromPoint(spawnPoints[currentSpawnIndex]);
            currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;
            return;
        }

        FireFromPoint(spawnPoints[0]);
    }

    private void FireFromPoint(Transform spawnPoint)
    {
        if (projectilePrefab == null || spawnPoint == null) return;

        TrapProjectile projectile = Instantiate(
            projectilePrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        Vector3 direction;

        if (targetPoint != null)
        {
            direction = (targetPoint.position - spawnPoint.position).normalized;
        }
        else
        {
            direction = spawnPoint.forward;
        }

        projectile.Initialize(direction);
    }
}