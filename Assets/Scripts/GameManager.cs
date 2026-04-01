using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI")]
    public TextMeshProUGUI deathsText;

    [Header("Spawn")]
    public Transform currentSpawnPoint;

    private int deathCount = 0;
    private int currentCheckpointIndex = -1;

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
}