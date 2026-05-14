using UnityEngine;

public class TrapProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 10f;
    public float lifetime = 4f;

    private Vector3 moveDirection = Vector3.forward;

    public void Initialize(Vector3 direction)
    {
        moveDirection = direction.normalized;
        transform.forward = moveDirection;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        transform.position += moveDirection * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.RegisterDeath(other.gameObject);
            Destroy(gameObject);
            return;
        }

        // opcjonalnie: ignoruj checkpointy i inne triggery
        if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}