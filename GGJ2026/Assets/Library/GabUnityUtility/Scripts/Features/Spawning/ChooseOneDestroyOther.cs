using System.Collections.Generic;
using UnityEngine;

public class ChooseOneDestroyOther : MonoBehaviour
{
    [SerializeField] private List<GameObject> gameObjects;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int chosen = Random.Range(0, gameObjects.Count);

        for (int i = 0; i < gameObjects.Count; i++)
        {
            if(i == chosen)
                continue;

            Destroy(gameObjects[i].gameObject);
        }
    }
}
