using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaskableCube), typeof(Collider))]
public class MaskableCube : MonoBehaviour
{
    public static List<MaskableCube> AllSelectables = new List<MaskableCube>();
    void OnEnable() => AllSelectables.Add(this);
    void OnDisable() => AllSelectables.Remove(this);

    [SerializeField] private Material defualt_mat;
    [SerializeField] private Material highlighted_mat;

    private Collider _collider;
    public Collider _Collider => _collider;

    private MeshRenderer meshRenderer;
    
    private void Awake()
    {
        _collider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();   
    }

    public void OnHover(bool what)
    {
        meshRenderer.sharedMaterial = what ? highlighted_mat : defualt_mat;
    }
}
