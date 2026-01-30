using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MaskableCube), typeof(Collider), typeof(MeshRenderer))]
public class MaskableCube : MonoBehaviour
{
    public static List<MaskableCube> AllSelectables = new List<MaskableCube>();
    void OnEnable() => AllSelectables.Add(this);
    void OnDisable() => AllSelectables.Remove(this);

    [SerializeField] private Material defualt_mat;
    [SerializeField] private Material highlighted_mat;
    [SerializeField] private float shader_t_speed = 1;
    [SerializeField] private string shader_t_name = "dissolve";
    private Collider _collider;
    public Collider _Collider => _collider;

    private MeshRenderer meshRenderer;

    private float shader_t = 0;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        meshRenderer = GetComponent<MeshRenderer>();   
    }

    public void OnHover(bool what)
    {
        meshRenderer.sharedMaterial = what ? highlighted_mat : defualt_mat;
    }

    public void CommitDisable()
    {
        _collider.enabled = false;
    }

    private void Update()
    {
        if (_collider.enabled)
        {
            shader_t += Time.deltaTime * shader_t_speed;
        }
        else
        {
            shader_t -= Time.deltaTime * shader_t_speed;
        }

        shader_t = Mathf.Clamp01(shader_t);

        meshRenderer.material.SetFloat(shader_t_name, shader_t);
    }
}
