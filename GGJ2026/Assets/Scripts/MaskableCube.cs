using UnityEngine;

[RequireComponent(typeof(MaskableCube), typeof(Collider))]
public class MaskableCube : MonoBehaviour
{
    [SerializeField] private Material defualt_mat;
    [SerializeField] private Material highlighted_mat;

    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();   
    }

    public void OnHover(bool what)
    {
        meshRenderer.sharedMaterial = what ? highlighted_mat : defualt_mat;
    }
}
