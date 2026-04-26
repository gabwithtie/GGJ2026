using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(SpriteRenderer))]
    class RandomSpriteOnStart : MonoBehaviour
    {
        [SerializeField] private Sprite[] sprites;
        private void Start()
        {
            if (sprites.Length == 0) return;
            GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }
}
