using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI deathsText;
    public TextMeshProUGUI timeText;

    [Header("Spawn")]
    public Transform currentSpawnPoint;

    private int deathCount = 0;
    private int currentCheckpointIndex = -1;
    private float elapsedTime = 0f;
    private bool timerRunning = true;

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

        UpdateDeathUI();
    }

    public void RegisterDeath(GameObject player)
    {
        deathCount++;
        UpdateDeathUI();

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            controller.enabled = false;
        }

        player.transform.position = currentSpawnPoint.position;

        if (controller != null)
        {
            controller.enabled = true;
        }

        FallingPlatform[] fallingPlatforms = FindObjectsByType<FallingPlatform>(FindObjectsSortMode.None);
        foreach (FallingPlatform platform in fallingPlatforms)
        {
            if (platform.resetOnPlayerRespawn)
            {
                platform.ResetPlatform();
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
    private void Update()
    {
        if (!timerRunning) return;

        elapsedTime += Time.deltaTime;
        UpdateTimeUI();
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