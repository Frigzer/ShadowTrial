using UnityEngine;

public class ProjectileTrapTrigger : MonoBehaviour
{
    public ProjectileTrap trap;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (trap != null)
        {
            trap.ActivateTrap();
        }
    }
}