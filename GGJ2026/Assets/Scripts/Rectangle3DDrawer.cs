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

        [Header("Events")]
        public UnityEvent<List<Collider>> OnSelectionReleased;

        private Vector2 startMousePos;
        private bool isSelecting = false;
        private Camera cam;

        void Start()
        {
            cam = Camera.main;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isSelecting = true;
                startMousePos = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0) && isSelecting)
            {
                isSelecting = false;
                List<Collider> selectedColliders = GetCollidersInSelection();

                if (selectedColliders.Count > 0)
                {
                    OnSelectionReleased.Invoke(selectedColliders);
                }
            }
        }

        private List<Collider> GetCollidersInSelection()
        {
            List<Collider> results = new List<Collider>();
            Rect selectionRect = GetScreenRect(startMousePos, Input.mousePosition);

            // Find all colliders in the world that could potentially be selected
            // You can optimize this by only checking colliders within a certain distance
            Collider[] allColliders = Physics.OverlapSphere(cam.transform.position, 100f, selectableLayer);

            foreach (var col in allColliders)
            {
                // Convert the world position of the object to screen space
                Vector3 screenPos = cam.WorldToScreenPoint(col.transform.position);

                // If the screen point is inside our selection rectangle, add it to the list
                // Note: WorldToScreenPoint returns Y from bottom, GUI uses Y from top. 
                // We stay consistent with Screen Space (Y bottom-up) here.
                if (selectionRect.Contains(screenPos))
                {
                    results.Add(col);
                }
            }

            return results;
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
                // GUI space has (0,0) at top-left, Screen space has (0,0) at bottom-left
                // We must flip the Y coordinate for drawing
                var rect = GetScreenRect(startMousePos, Input.mousePosition);
                Rect guiRect = new Rect(rect.x, Screen.height - rect.yMax, rect.width, rect.height);

                // Draw the box background
                GUI.color = selectionColor;
                GUI.DrawTexture(guiRect, Texture2D.whiteTexture);

                // Draw the box border
                GUI.color = borderColor;
                DrawRectBorder(guiRect, 2f);

                GUI.color = Color.white;
            }
        }

        private void DrawRectBorder(Rect rect, float thickness)
        {
            // Top
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), Texture2D.whiteTexture);
            // Bottom
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), Texture2D.whiteTexture);
            // Left
            GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), Texture2D.whiteTexture);
            // Right
            GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), Texture2D.whiteTexture);
        }
    }
}