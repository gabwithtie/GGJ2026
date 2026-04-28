using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Renderer))]
    public class MaterialPropertyController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int targetMaterialIndex = 0;
        [SerializeField] private string propertyName = "_FloatValue";

        private Renderer _renderer;
        private MaterialPropertyBlock _propBlock;
        private int _propertyID;
        private float _valueCache;

        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _propBlock = new MaterialPropertyBlock();

            // Convert the string name to a unique integer ID
            _propertyID = Shader.PropertyToID(propertyName);
        }

        /// <summary>
        /// Public function to update the float property from other scripts or UI events.
        /// </summary>
        public void UpdateFloatProperty(float newValue)
        {
            SetOnly(newValue);
            CommitValue();
        }

        public void SetOnly(float newValue)
        {
            _valueCache = newValue;
        }

        public void CommitValue()
        {
            if (_renderer == null) return;

            // 1. Get current properties from the renderer into the block
            _renderer.GetPropertyBlock(_propBlock, targetMaterialIndex);

            // 2. Assign our new value to the block
            _propBlock.SetFloat(_propertyID, _valueCache);

            // 3. Push the block back to the renderer
            _renderer.SetPropertyBlock(_propBlock, targetMaterialIndex);
        }
    }
}