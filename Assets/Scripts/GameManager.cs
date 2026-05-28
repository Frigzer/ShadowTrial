using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI timeText;
    public GameObject deathPanel;

    [Header("Respawn")]
    public Transform currentSpawnPoint;
    public bool autoRespawn = true;
    public float autoRespawnDelay = 1.5f;

    private int deathCount = 0;
    private int currentCheckpointIndex = -1;
    private float elapsedTime = 0f;
    private bool timerRunning = true;
    private bool isDead = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        UpdateDeathUI();
        UpdateTimeUI();
    }

    private void Update()
    {
        if (!timerRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimeUI();
    }

    public void RegisterDeath(GameObject player)
    {
        if (isDead) return;

        StartCoroutine(DeathSequence(player));
    }

    private IEnumerator DeathSequence(GameObject player)
    {
        isDead = true;
        timerRunning = false;

        deathCount++;
        UpdateDeathUI();

        CharacterController controller = player.GetComponent<CharacterController>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        if (movement != null)
        {
            movement.enabled = false;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(true);
        }

        if (autoRespawn)
        {
            yield return new WaitForSeconds(autoRespawnDelay);
            RespawnPlayer(player);
        }
    }

    public void RespawnPlayer(GameObject player)
    {
        CharacterController controller = player.GetComponent<CharacterController>();
        PlayerMovement movement = player.GetComponent<PlayerMovement>();

        if (controller != null)
        {
            controller.enabled = false;
        }

        player.transform.position = currentSpawnPoint.position;

        ResetRespawnObjects();

        if (controller != null)
        {
            controller.enabled = true;
        }

        if (movement != null)
        {
            movement.enabled = true;
        }

        if (deathPanel != null)
        {
            deathPanel.SetActive(false);
        }

        timerRunning = true;
        isDead = false;
    }

    public void RespawnPlayerFromButton()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            RespawnPlayer(player);
        }
    }

    private void ResetRespawnObjects()
    {
        FallingPlatform[] fallingPlatforms = FindObjectsByType<FallingPlatform>(FindObjectsSortMode.None);
        foreach (FallingPlatform platform in fallingPlatforms)
        {
            if (platform.resetOnPlayerRespawn)
            {
                platform.ResetPlatform();
            }
        }

        PushingWall[] pushingWalls = FindObjectsByType<PushingWall>(FindObjectsSortMode.None);
        foreach (PushingWall wall in pushingWalls)
        {
            if (wall.resetOnPlayerRespawn)
            {
                wall.ResetWall();
            }
        }
    }

    public void TrySetCheckpoint(Transform newSpawn, int checkpointIndex)
    {
        if (checkpointIndex > currentCheckpointIndex)
        {
            currentCheckpointIndex = checkpointIndex;
            currentSpawnPoint = newSpawn;
            Debug.Log("Activated checkpoint: " + checkpointIndex);
        }
    }

    private void UpdateDeathUI()
    {
        if (deathsText != null)
        {
            deathsText.text = "Deaths: " + deathCount;
        }
    }

    private void UpdateTimeUI()
    {
        if (timeText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);

        timeText.text = $"Time: {minutes:00}:{seconds:00}.{milliseconds:00}";
    }
}