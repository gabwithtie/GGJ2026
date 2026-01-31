using UnityEngine;

namespace GabUnity
{
    public class GridGenerator : MonoBehaviour
    {
        [Header("Prefab Settings")]
        [SerializeField] private GameObject prefab;

        [Header("Grid Dimensions")]
        [SerializeField] private int width = 10;   // X Axis
        [SerializeField] private int height = 1;   // Y Axis
        [SerializeField] private int depth = 10;   // Z Axis

        [Header("Spacing Settings")]
        [SerializeField] private float spacing = 1.0f;
        [SerializeField] private bool centerGrid = true;

        [Header("Visualization")]
        [SerializeField] private Color gizmoColor = Color.cyan;
        [SerializeField] private bool showIndividualNodes = true;

        private void Start()
        {
            GenerateGrid();
        }

        public void GenerateGrid()
        {
            if (prefab == null) return;

            Vector3 offset = GetCenterOffset();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        Vector3 localPos = new Vector3(x * spacing, y * spacing, z * spacing) - offset;
                        Vector3 worldPos = transform.TransformPoint(localPos);

                        GameObject instance = Instantiate(prefab, worldPos, transform.rotation);
                        instance.transform.SetParent(this.transform);
                        var scale = Vector3.one * spacing;
                        scale.z = 1;
                        instance.transform.localScale = scale;
                        instance.name = $"Cube_{x}_{y}_{z}";
                    }
                }
            }
        }

        private Vector3 GetCenterOffset()
        {
            if (!centerGrid) return Vector3.zero;
            return new Vector3((width - 1) * spacing / 2f, (height - 1) * spacing / 2f, (depth - 1) * spacing / 2f);
        }

        private void OnDrawGizmos()
        {
            // Calculate the total size of the grid volume
            Vector3 size = new Vector3((width - 1) * spacing, (height - 1) * spacing, (depth - 1) * spacing);

            // The center of the Gizmo box depends on whether we are centering the grid or not
            Vector3 localCenter = centerGrid ? Vector3.zero : size / 2f;

            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = gizmoColor;

            // Draw the outer boundary box
            Gizmos.DrawWireCube(localCenter, size + (Vector3.one * 0.1f));

            if (showIndividualNodes)
            {
                Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 0.3f);
                Vector3 offset = GetCenterOffset();

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int z = 0; z < depth; z++)
                        {
                            Vector3 nodePos = new Vector3(x * spacing, y * spacing, z * spacing) - offset;
                            Gizmos.DrawSphere(nodePos, 0.1f * spacing);
                        }
                    }
                }
            }
        }
    }
}