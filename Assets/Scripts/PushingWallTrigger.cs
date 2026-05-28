using UnityEngine;

public class PushingWallTrigger : MonoBehaviour
{
    public PushingWall targetWall;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (targetWall == null) return;

        targetWall.ActivateWall();
    }
}