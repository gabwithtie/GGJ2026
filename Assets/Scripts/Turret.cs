using GabUnity;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(UnitIdentifier))]
public class Turret : MonoBehaviour
{
    private ClosestUnitSelector targetSelector;
    private UnitIdentifier unitIdentifier;

    [Header("Turret")]
    [SerializeField] private float fire_rate = 1f;
    [SerializeField] private int burst_count = 3;
    [SerializeField] private float burst_interval = 2.0f;
    [SerializeField] private float damage = 1f;
    [SerializeField] private float projectile_speed = 1f;
    [SerializeField] private string projectile_id = "";
    [SerializeField] private Vector3 aimOffset;

    [Header("References")]
    [SerializeField] private Transform Y_rot_barrel;
    [SerializeField] private Transform X_rot_barrel;
    [SerializeField] private Transform barrel_tip;
    [SerializeField] private UnityEvent onShoot;

    private float time_since_last_shot = 0f;
    private float time_since_last_burst = 0f;
    private int shots_in_curburst = 0;

    private void Awake()
    {
        unitIdentifier = GetComponent<UnitIdentifier>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetSelector = GetComponentInChildren<ClosestUnitSelector>();
    }

    private void OnEnable()
    {
        time_since_last_shot = -0.1f; // Small delay to allow target acquisition
    }

    // Update is called once per frame
    void Update()
    {
        time_since_last_shot += Time.deltaTime;
        time_since_last_burst += Time.deltaTime;

        if (time_since_last_burst > burst_interval)
        {
            time_since_last_burst = 0;
            shots_in_curburst = 0;
        }

        var point_at = targetSelector.Closest;

        if (point_at == null)
        {
            return;
        }

        Vector3 direction_to_target = (point_at.transform.position + aimOffset) - X_rot_barrel.position;

        // Y axis rotation
        if (Y_rot_barrel != null)
        {
            Vector3 y_direction = new Vector3(direction_to_target.x, 0, direction_to_target.z);
            Quaternion y_target_rotation = Quaternion.LookRotation(y_direction);
            Y_rot_barrel.rotation = Quaternion.Slerp(Y_rot_barrel.rotation, y_target_rotation, Time.deltaTime * 5f);
        }
        // X axis rotation
        if (X_rot_barrel != null)
        {
            Vector3 x_direction = Y_rot_barrel.InverseTransformDirection(direction_to_target);
            Quaternion x_target_rotation = Quaternion.LookRotation(x_direction);
            X_rot_barrel.localRotation = Quaternion.Slerp(X_rot_barrel.localRotation, x_target_rotation, Time.deltaTime * 5f);
        }

        if (fire_rate < 0.01)
            return;

        if (shots_in_curburst < burst_count)
        {
            if (time_since_last_shot >= 1f / fire_rate)
            {
                time_since_last_shot = 0f;
                ShootAt(point_at);
                shots_in_curburst++;
            }
        }
    }

    private void ShootAt(UnitIdentifier target)
    {
        var shootinfo = new ProjectileController.ShootInfo(unitIdentifier)
        {
            damage = damage,
            projectile_speed = projectile_speed
        };

        var projectile = ProjectileManager.Shoot(barrel_tip.position, barrel_tip.forward, shootinfo, projectile_id);
        onShoot.Invoke();
    }
}
