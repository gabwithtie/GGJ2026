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

        [Header("Cost Settings")]
        [SerializeField] private float maxScreenCost = 2; // Cost if 100% of screen is selected
        [SerializeField] private UnityEvent<float> OnChangeCost;

        private Vector2 startMousePos;
        private bool isSelecting = false;
        private Camera cam;
        private float currentSelectionCost;

        private List<MaskableCube> selectedColliders = new List<MaskableCube>();

        void Start() => cam = Camera.main;

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                isSelecting = true;
                startMousePos = Input.mousePosition;
            }

            if (isSelecting)
            {
                UpdateCollidersInSelection();
                currentSelectionCost = CalculateNormalizedCost();
            }

            if (Input.GetMouseButtonUp(0) && isSelecting)
            {
                isSelecting = false;

                if (EnergyManager.TryUseEnergy(currentSelectionCost))
                    foreach (var cube in selectedColliders) Destroy(cube.gameObject);
                else
                    foreach (var cube in selectedColliders) cube.OnHover(false);

                selectedColliders.Clear();
                currentSelectionCost = 0;
            }

            OnChangeCost.Invoke(currentSelectionCost);
        }

        private float CalculateNormalizedCost()
        {
            Rect rect = GetScreenRect(startMousePos, Input.mousePosition);

            // Calculate area of selection box in pixels
            float selectionArea = rect.width * rect.height;

            // Calculate total screen area in pixels
            float screenArea = Screen.width * Screen.height;

            // Normalize (0.0 to 1.0) and multiply by maxCost
            float normalizedArea = selectionArea / screenArea;
            return normalizedArea * maxScreenCost;
        }

        private void UpdateCollidersInSelection()
        {
            Rect selectionRect = GetScreenRect(startMousePos, Input.mousePosition);
            HashSet<MaskableCube> currentFrameInside = new HashSet<MaskableCube>();
            var allSelectables = MaskableCube.AllSelectables;

            for (int i = 0; i < allSelectables.Count; i++)
            {
                MaskableCube col = allSelectables[i];
                if (col == null) continue;

                Vector3 screenPos = cam.WorldToScreenPoint(col._Collider.bounds.center);

                if (screenPos.z < 0 || !selectionRect.Contains(screenPos))
                    continue;

                // Occlusion check
                var dir = col.transform.position - cam.transform.position;
                if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, Mathf.Infinity, selectableLayer, QueryTriggerInteraction.Collide))
                {
                    if (hit.collider != col._Collider)
                        continue;
                }

                currentFrameInside.Add(col);
            }

            // Handle ENTER/EXIT logic
            for (int i = selectedColliders.Count - 1; i >= 0; i--)
            {
                if (!currentFrameInside.Contains(selectedColliders[i]))
                {
                    selectedColliders[i].OnHover(false);
                    selectedColliders.RemoveAt(i);
                }
            }

            foreach (var col in currentFrameInside)
            {
                if (!selectedColliders.Contains(col))
                {
                    selectedColliders.Add(col);
                    col.OnHover(true);
                }
            }
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
                Rect guiRect = new Rect(rect.x, Screen.height - rect.yMax, rect.width, rect.height);

                // Visual feedback: change box color if too expensive
                GUI.color = (EnergyManager.CurEnergy >= currentSelectionCost) ? selectionColor : new Color(1, 0, 0, 0.25f);
                GUI.DrawTexture(guiRect, Texture2D.whiteTexture);

                GUI.color = borderColor;
                DrawRectBorder(guiRect, 2f);

                // Show cost label next to mouse
                GUI.Label(new Rect(Input.mousePosition.x + 10, Screen.height - Input.mousePosition.y, 150, 20),
                    $"Cost: {currentSelectionCost:F0} / {EnergyManager.CurEnergy:F0}");

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