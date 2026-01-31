using NUnit.Framework;
using System.Linq;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(LineRenderer))]
    public class TargettedLine : MonoBehaviour
    {
        private LineRenderer lineRenderer;

        [SerializeField] private Transform point_a;
        [SerializeField] private Transform point_b;

        [SerializeField] private bool use_raycast = false;
        [SerializeField] private Collider[] ignorecolliders;
        [SerializeField] private string global_effect_end = "";
        [SerializeField] private float global_effect_interval = 0.05f;

        float time_last_spawned = 0f;
        private RaycastHit[] hitinfos = new RaycastHit[3];

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        public void SetEnd(UnitIdentifier point)
        {
            if(point == null)
            {
                point_b = null;
                return;
            }

            point_b = point.transform;
        }
        public void SetEnd(Transform point) => point_b = point;

        // Update is called once per frame
        void Update()
        {
            if (point_a == null)
            {
                lineRenderer.enabled = false;
                return;
            }

            if (point_b == null)
            {
                lineRenderer.enabled = false;
                return;
            }

            if(lineRenderer.enabled == false)
            {
                lineRenderer.enabled = true;
            }
            
            if (lineRenderer.positionCount != 2)
            {
                lineRenderer.positionCount = 2;
            }

            lineRenderer.SetPosition(0, point_a.position);
            lineRenderer.SetPosition(1, point_b.position);

            if(global_effect_end.Length > 0)
            {
                time_last_spawned += Time.deltaTime;

                if(time_last_spawned > global_effect_interval)
                {
                    var end_pos = point_b.position;
                    var normal = Vector3.up;
                    time_last_spawned = 0f;

                    if (use_raycast)
                    {
                        var hitsmth = Physics.RaycastNonAlloc(new Ray(point_a.position, point_b.position - point_a.position), hitinfos);

                        if (hitsmth > 0)
                        {
                            foreach (var hitinfo in hitinfos)
                            {
                                if (ignorecolliders.Contains(hitinfo.collider))
                                    continue;

                                end_pos = hitinfo.point;
                                normal = hitinfo.normal;

                                break;
                            }
                        }
                    }

                    GlobalEffectManager.Spawn(global_effect_end, end_pos, normal);
                }
            }
        }
    }
}