using UnityEngine;
using Parabox.CSG;
using System.Collections.Generic;
using System.Linq;

namespace GabUnity
{
    public class MeshEraser : MonoBehaviour
    {
        [SerializeField] private GameObject targetObject;
        [SerializeField] private float maxDepth = 50f;

        private Vector3 startMousePos;
        private bool isSelecting = false;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                startMousePos = Input.mousePosition;
                isSelecting = true;
            }

            if (Input.GetMouseButtonUp(0) && isSelecting)
            {
                isSelecting = false;
                CreateProjectedHole(startMousePos, Input.mousePosition);
            }
        }

        private void CreateProjectedHole(Vector2 screenStart, Vector2 screenEnd)
        {
            if (targetObject == null) return;

            Camera cam = Camera.main;
            Rect rect = GetScreenRect(screenStart, screenEnd);

            float near = 0.1f;
            float far = maxDepth;

            Vector3[] verts = new Vector3[8];
            verts[0] = cam.ScreenToWorldPoint(new Vector3(rect.xMin, rect.yMin, near));
            verts[1] = cam.ScreenToWorldPoint(new Vector3(rect.xMax, rect.yMin, near));
            verts[2] = cam.ScreenToWorldPoint(new Vector3(rect.xMax, rect.yMax, near));
            verts[3] = cam.ScreenToWorldPoint(new Vector3(rect.xMin, rect.yMax, near));
            verts[4] = cam.ScreenToWorldPoint(new Vector3(rect.xMin, rect.yMin, far));
            verts[5] = cam.ScreenToWorldPoint(new Vector3(rect.xMax, rect.yMin, far));
            verts[6] = cam.ScreenToWorldPoint(new Vector3(rect.xMax, rect.yMax, far));
            verts[7] = cam.ScreenToWorldPoint(new Vector3(rect.xMin, rect.yMax, far));

            GameObject cutter = CreateFrustumCutter(verts);

            // Perform the Boolean Operation
            Model result = CSG.Subtract(targetObject, cutter);

            // --- REVERT TRANSFORMATION LOGIC ---

            // 1. Get the world-space mesh from the result
            Mesh bakedMesh = result.mesh;
            Vector3[] localVertices = bakedMesh.vertices;

            // 2. Convert every vertex from World Space back to the Target's Local Space
            // This 'un-bakes' the transform that Parabox added.
            for (int i = 0; i < localVertices.Length; i++)
            {
                localVertices[i] = targetObject.transform.InverseTransformPoint(localVertices[i]);
            }

            // 3. Update the mesh data
            bakedMesh.vertices = localVertices;
            bakedMesh.RecalculateBounds();

            // 4. Assign the corrected mesh back
            targetObject.GetComponent<MeshFilter>().sharedMesh = bakedMesh;
            targetObject.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();

            if (targetObject.TryGetComponent<MeshCollider>(out var col))
            {
                col.sharedMesh = bakedMesh;
            }

            Destroy(cutter);
        }

        private GameObject CreateFrustumCutter(Vector3[] corners)
        {
            GameObject go = new GameObject("TempCutter");
            MeshFilter mf = go.AddComponent<MeshFilter>();
            Mesh m = new Mesh();

            m.vertices = corners;
            m.triangles = new int[]
            {
                0, 2, 1, 0, 3, 2, // Near
                4, 5, 6, 4, 6, 7, // Far
                0, 1, 5, 0, 5, 4, // Bottom
                1, 2, 6, 1, 6, 5, // Right
                2, 3, 7, 2, 7, 6, // Top
                3, 0, 4, 3, 4, 7  // Left
            };

            m.RecalculateNormals();
            mf.sharedMesh = m;
            go.AddComponent<MeshRenderer>();

            return go;
        }

        private Rect GetScreenRect(Vector2 start, Vector2 end)
        {
            return Rect.MinMaxRect(
                Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y),
                Mathf.Max(start.x, end.x), Mathf.Max(start.y, end.y)
            );
        }

        private void OnGUI()
        {
            if (isSelecting)
            {
                var rect = GetScreenRect(startMousePos, Input.mousePosition);
                GUI.Box(new Rect(rect.x, Screen.height - rect.yMax, rect.width, rect.height), "Selecting Area...");
            }
        }
    }
}