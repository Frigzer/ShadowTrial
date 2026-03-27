using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class KillZoneGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        BoxCollider box = GetComponent<BoxCollider>();

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);

        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawCube(box.center, box.size);

        Gizmos.color = new Color(1f, 0f, 0f, 0.9f);
        Gizmos.DrawWireCube(box.center, box.size);

        Gizmos.matrix = oldMatrix;
    }
}