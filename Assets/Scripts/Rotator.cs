using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Rotation")]
    public Vector3 rotationAxis = Vector3.up;
    public float rotationSpeed = 45f;

    [Header("Space")]
    public bool useLocalSpace = true;

    private void Update()
    {
        Space space = useLocalSpace ? Space.Self : Space.World;
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, space);
    }
}