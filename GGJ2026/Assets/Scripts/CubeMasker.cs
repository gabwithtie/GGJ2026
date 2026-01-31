using System.Collections.Generic;
using UnityEngine;

public class CubeMasker : MonoBehaviour
{
    public void OnEnter(Collider other)
    {
        if (other.TryGetComponent(out MaskableCube maskableCube))
        {
            
        }
    }

    public void OnExit(Collider other)
    {
        if (other.TryGetComponent(out MaskableCube maskableCube))
        {
            maskableCube.OnHover(false);
        }
    }

    public void CommitMask(List<Collider> list)
    {
        foreach (var cube in list)
        {
            if (cube.TryGetComponent(out MaskableCube _))
            {
                Destroy(cube.gameObject);
            }
        }
    }
}
