using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ObjectGizmoRenderer : MonoBehaviour
{
#if UNITY_EDITOR
    private enum ShapeType
    {
        Cube,
        Sphere,
        CustomMesh
    }

    [Header("Gizmo Settings")] [SerializeField]
    private bool _drawOnlyWireframe;

    [SerializeField] private ShapeType _shape = ShapeType.Cube;
    [SerializeField] private Color _gizmoColor = Color.gray;

    [Header("Custom Mesh")] [SerializeField]
    private Mesh _customMesh;
    [SerializeField] private int _currentMeshIndex = -1;

    private void OnDrawGizmos()
    {
        var size = transform.localScale;
        Gizmos.color = _gizmoColor;

        switch (_shape)
        {
            case ShapeType.Cube:
                if (_drawOnlyWireframe)
                    Gizmos.DrawWireCube(transform.position, transform.localScale);
                else
                    Gizmos.DrawCube(transform.position, transform.localScale);
                break;

            case ShapeType.Sphere:
                float radius = (size.x + size.y + size.z) / 3f * 0.5f;
                if (_drawOnlyWireframe)
                    Gizmos.DrawWireSphere(transform.position, radius);
                else
                    Gizmos.DrawSphere(transform.position, radius);
                break;

            case ShapeType.CustomMesh:
                if (_customMesh)
                {
                    if (_drawOnlyWireframe)
                        Gizmos.DrawWireMesh(_customMesh, _currentMeshIndex, transform.position, transform.rotation,
                            size);
                    else
                        Gizmos.DrawMesh(_customMesh, _currentMeshIndex, transform.position, transform.rotation, size);
                }

                break;
        }
    }

    private void OnValidate()
    {
        SceneView.RepaintAll();
    }
#endif
}