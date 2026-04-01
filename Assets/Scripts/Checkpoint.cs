using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointIndex = 0;
    public Transform spawnPoint;
    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.Instance.TrySetCheckpoint(spawnPoint, checkpointIndex);

        if (!activated)
        {
            activated = true;
            Debug.Log("Checkpoint reached: " + checkpointIndex);
        }
    }
}