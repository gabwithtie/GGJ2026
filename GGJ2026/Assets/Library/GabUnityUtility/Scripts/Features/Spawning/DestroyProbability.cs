using System.Collections.Generic;
using UnityEngine;

public class DestroyProbability : MonoBehaviour
{
    [SerializeField] private float probability;

    [SerializeField] private DestroyProbability mutuallyExclusiveTo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(mutuallyExclusiveTo != null)
            Destroy(this.gameObject);

        if (Random.Range(0.0f, 1.0f) < probability)
            Destroy(this.gameObject);
    }
}
