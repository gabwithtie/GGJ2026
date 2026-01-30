using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    public class Rectangle3DDrawer : MonoBehaviour
    {
        [Header("Selection Settings")]
        [SerializeField] private LayerMask selectableLayer;
        [SerializeField] private Color selectionColor = new Color(0, 1, 0, 0.25f);
        [SerializeField] private Color borderColor = Color.green;
        [SerializeField] private float detectionRadius = 150f; // Increased for safety

        [Header("Events")]
        public UnityEvent<List<Collider>> OnSelectionReleased;
        public UnityEvent<Collider> OnSelectionEnter;
        public UnityEvent<Collider> OnSelectionExit;

        private Vector2 startMousePos;
        private bool isSelecting = false;
        private Camera cam;

        private List<Collider> selectedColliders = new List<Collider>();
        private Collider[] colcache = new Collider[512]; // Increased buffer size

        void Start() => cam = Camera.main;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isSelecting = true;
                startMousePos = Input.mousePosition;
                // Don't clear here if you want to keep highlights until release
            }

            if (isSelecting) UpdateCollidersInSelection();

            if (Input.GetMouseButtonUp(0) && isSelecting)
            {
                isSelecting = false;
                if (selectedColliders.Count > 0)
                    OnSelectionReleased.Invoke(new List<Collider>(selectedColliders));

                // Clear the state for the next drag
                selectedColliders.Clear();
            }
        }

        private void UpdateCollidersInSelection()
        {
            Rect selectionRect = GetScreenRect(startMousePos, Input.mousePosition);

            // 1. Get everything in a large radius around the camera
            int total_in = Physics.OverlapSphereNonAlloc(cam.transform.position, detectionRadius, colcache, selectableLayer);

            // 2. Determine what is inside the 2D Rect THIS frame
            HashSet<Collider> currentFrameInside = new HashSet<Collider>();
            for (int i = 0; i < total_in; i++)
            {
                var col = colcache[i];
                if (col == null) continue; // Safety check

                Vector3 screenPos = cam.WorldToScreenPoint(col.bounds.center); // Use bounds.center for better accuracy

                if (screenPos.z > 0 && selectionRect.Contains(screenPos))
                {
                    currentFrameInside.Add(col);
                }
            }

            // 3. EXIT Logic: Was in the list, but not in the rect anymore
            for (int i = selectedColliders.Count - 1; i >= 0; i--)
            {
                if (!currentFrameInside.Contains(selectedColliders[i]))
                {
                    OnSelectionExit.Invoke(selectedColliders[i]);
                    selectedColliders.RemoveAt(i);
                }
            }

            // 4. ENTER Logic: In the rect, but not in our list yet
            foreach (var col in currentFrameInside)
            {
                if (!selectedColliders.Contains(col))
                {
                    selectedColliders.Add(col);
                    OnSelectionEnter.Invoke(col);
                }
            }

            // 5. CRITICAL: Clear the cache array references for the next frame 
            // to avoid holding onto destroyed objects or old references
            System.Array.Clear(colcache, 0, total_in);
        }

        // ... [Keep GetScreenRect, OnGUI, and DrawRectBorder as they were] ...
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
                Rect guiRect = new Rect(rect.x, Screen.height - rect.yMax, rect.width, rect.height);
                GUI.color = selectionColor;
                GUI.DrawTexture(guiRect, Texture2D.whiteTexture);
                GUI.color = borderColor;
                DrawRectBorder(guiRect, 2f);
                GUI.color = Color.white;
            }
        }

        private void DrawRectBorder(Rect rect, float thickness)
        {
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), Texture2D.whiteTexture);
            GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), Texture2D.whiteTexture);
        }
    }
}