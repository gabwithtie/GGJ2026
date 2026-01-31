using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class NavmeshPhysicsAgent : MonoBehaviour
{
    private NavMeshAgent mNavMesh;
    private Rigidbody m_rigidbody;

    [SerializeField] private Vector3 target;
    [SerializeField] private Vector3 current_direction;
    [SerializeField] private float force;

    public void SetTarget(Vector3 value) => target = value;
    public Vector3 CurrentDirection => current_direction;
    public bool HasDirectVision => cachedcorners == null || cachedcorners.Length <= 2;
    private Vector3[] cachedcorners;

    private Vector3 oldtarget_pos;
    private bool recalculate;

    private void Awake()
    {
        mNavMesh = GetComponent<NavMeshAgent>();
        m_rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {

        if ((target - oldtarget_pos).sqrMagnitude > 0.25)
            recalculate = true;

        if (recalculate)
        {
            oldtarget_pos = target;

            if (this.gameObject.activeInHierarchy == false)
                return;
            if (mNavMesh.SetDestination(target) == false)
                return;

            cachedcorners = mNavMesh.path.corners;
        }

        if (cachedcorners == null)
            return;

        if (cachedcorners.Length > 2)
        {
            var delta = cachedcorners[1] - this.transform.position;
            var delta_norm = delta.normalized;

            if (delta.sqrMagnitude < 0.25f)
                recalculate = true;

            current_direction = delta_norm;
        }
        else if(cachedcorners.Length == 2)
        {
            current_direction = cachedcorners[1] - this.transform.position;
        }

        m_rigidbody.AddForce(current_direction * force, ForceMode.VelocityChange);
    }

    private void OnDrawGizmos()
    {

        if (cachedcorners == null || cachedcorners.Length == 0)
            return;

        Vector3 previous_pos = cachedcorners[0];

        foreach (var vertex in cachedcorners)
        {
            Gizmos.DrawLine(previous_pos, vertex);
            previous_pos = vertex;
        }
    }
}
