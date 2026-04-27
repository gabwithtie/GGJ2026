using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    public class PrefabOnLoadChooser : MonoBehaviour
    {
        [SerializeField] private List<GameObject> choices;

        private void Start()
        {
            var chosen = choices[Random.Range(0, choices.Count)];

            var newobj = Instantiate(chosen, this.transform);
        }
    }
}