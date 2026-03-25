using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillZone"))
        {
            GameManager.Instance.RegisterDeath(gameObject);
        }
    }
}