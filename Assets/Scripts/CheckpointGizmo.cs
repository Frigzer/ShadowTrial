using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckpointGizmo : MonoBehaviour
{
    [SerializeField] private Color fillColor = new Color(0f, 1f, 0f, 0.25f);
    [SerializeField] private Color wireColor = new Color(0f, 1f, 0f, 0.9f);

    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = fillColor;
        Gizmos.DrawCube(box.center, box.size);

        Gizmos.color = wireColor;
        Gizmos.DrawWireCube(box.center, box.size);

        Gizmos.matrix = oldMatrix;
    }
}