using System.Collections.Generic;
using UnityEngine;

public class CubeMasker : MonoBehaviour
{
    private List<MaskableCube> cubesinside = new();

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out MaskableCube maskableCube))
        {
            if (cubesinside.Contains(maskableCube))
                return;

            cubesinside.Add(maskableCube);
            maskableCube.OnHover(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out MaskableCube maskableCube))
        {
            cubesinside.Remove(maskableCube);
            maskableCube.OnHover(false);
        }
    }

    public void CommitMask()
    {
        foreach (var cube in cubesinside)
        {
            Destroy(cube.gameObject);
        }

        cubesinside.Clear();
    }
}
