using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    [CreateAssetMenu(menuName = "GabUnity/Dictionary/Generic List")]
    public class GenericListAsset : ScriptableObject
    {
        [SerializeField] private List<GameObject> items;
        public List<GameObject> Items => items;

        public GameObject GetRandom()
        {
            return items[Random.Range(0, items.Count)];
        }
    }
}
