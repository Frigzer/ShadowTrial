using UnityEngine;

public class TrapActivationZone : MonoBehaviour
{
    [Header("Traps")]
    public ProjectileTrap[] trapsToActivate;
    public ProjectileTrap[] trapsToDeactivate;

    [Header("Behavior")]
    public bool activateOnlyOnce = true;

    private bool used = false;

    private void OnTriggerEnter(Collider other)
    {
        if (used && activateOnlyOnce) return;
        if (!other.CompareTag("Player")) return;

        foreach (ProjectileTrap trap in trapsToActivate)
        {
            if (trap != null)
            {
                trap.ActivateTrap();
            }
        }

        foreach (ProjectileTrap trap in trapsToDeactivate)
        {
            if (trap != null)
            {
                trap.DeactivateTrap();
            }
        }

        used = true;
    }
}