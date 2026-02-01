using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace GabUnity
{
    public class Rectangle3DDrawer : MonoBehaviour
    {
        [Header("Selection Settings")]
        [SerializeField] private LayerMask selectableLayer;
        [SerializeField] private Color selectionColor = new Color(0, 1, 0, 0.25f);
        [SerializeField] private Color borderColor = Color.green;

        [Header("Cost Settings")]
        [SerializeField] private float cost_per_block = 0.05f;
        [SerializeField] private float holdCost_persecond = 2;
        [SerializeField] private UnityEvent<float> OnChangeCost;
        [SerializeField] private UnityEvent OnStartDrag;
        [SerializeField] private UnityEvent OnEndDrag;
        [SerializeField] private UnityEvent OnEndDrag_fail;
        [SerializeField] private UnityEvent OnEndDrag_success;

        private Vector2 startMousePos;
        private Vector2 currentMousePos;
        private bool isSelecting = false;
        private Camera cam;
        private float currentSelectionCost;

        private List<MaskableCube> selectedColliders = new List<MaskableCube>();

        void Start() => cam = Camera.main;

        // --- PUBLIC FUNCTIONS FOR PLAYER INPUT COMPONENT ---

        /// <summary>
        /// Hook this up to the "Position" action (Value, Vector2)
        /// </summary>
        public void OnPointerPosition(InputAction.CallbackContext context)
        {
            currentMousePos = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// Hook this up to the "Select" action (Button)
        /// </summary>
        public void OnSelectHold(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                isSelecting = true;
                startMousePos = currentMousePos;
                OnStartDrag.Invoke();
            }
            else if (context.canceled)
            {
                ProcessSelectionEnd();
            }
        }

        // --- LOGIC ---

        void Update()
        {
            if (isSelecting)
            {
                UpdateCollidersInSelection();
                currentSelectionCost = CalculateNormalizedCost();

                if (!EnergyManager.TryUseEnergy(Time.unscaledDeltaTime * holdCost_persecond))
                {
                    CancelSelection();
                }
            }

            OnChangeCost.Invoke(currentSelectionCost);
        }

        private void ProcessSelectionEnd()
        {
            if (!isSelecting) return;
            isSelecting = false;

            if (EnergyManager.TryUseEnergy(currentSelectionCost))
            {
                foreach (var cube in selectedColliders) cube.CommitEdit();
                OnEndDrag_success.Invoke();
            }
            else
            {
                foreach (var cube in selectedColliders) cube.OnHover(false);
                OnEndDrag_fail.Invoke();
            }

            selectedColliders.Clear();
            currentSelectionCost = 0;
            OnEndDrag.Invoke();
        }

        private void CancelSelection()
        {
            isSelecting = false;
            foreach (var cube in selectedColliders) cube.OnHover(false);
            OnEndDrag_fail.Invoke();
            selectedColliders.Clear();
            currentSelectionCost = 0;
            OnEndDrag.Invoke();
        }

        private float CalculateNormalizedCost()
        {
            return selectedColliders.Count * cost_per_block;
        }

        private void UpdateCollidersInSelection()
        {
            Rect selectionRect = GetScreenRect(startMousePos, currentMousePos);
            HashSet<MaskableCube> currentFrameInside = new HashSet<MaskableCube>();
            var allSelectables = MaskableCube.AllSelectables;

            for (int i = 0; i < allSelectables.Count; i++)
            {
                MaskableCube col = allSelectables[i];
                if (col == null) continue;
                if (col.Committed) continue;

                Vector3 screenPos = cam.WorldToScreenPoint(col._Collider.bounds.center);
                if (screenPos.z < 0 || !selectionRect.Contains(screenPos)) continue;

                var dir = col.transform.position - cam.transform.position;
                if (Physics.Raycast(cam.transform.position, dir, out RaycastHit hit, Mathf.Infinity, selectableLayer, QueryTriggerInteraction.Collide))
                {
                    var same_z = Mathf.Abs(hit.collider.transform.position.z - col.transform.position.z) < 0.3f;
                    if (hit.collider != col._Collider)
                        if (!same_z)
                            continue;
                }
                currentFrameInside.Add(col);
            }

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
                var rect = GetScreenRect(startMousePos, currentMousePos);
                Rect guiRect = new Rect(rect.x, Screen.height - rect.yMax, rect.width, rect.height);

                GUI.color = (EnergyManager.CurEnergy >= currentSelectionCost) ? selectionColor : new Color(1, 0, 0, 0.25f);
                GUI.DrawTexture(guiRect, Texture2D.whiteTexture);
                GUI.color = borderColor;
                DrawRectBorder(guiRect, 2f);

                GUI.Label(new Rect(currentMousePos.x + 10, Screen.height - currentMousePos.y, 150, 20),
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