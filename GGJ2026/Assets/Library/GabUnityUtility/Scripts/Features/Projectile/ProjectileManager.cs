using UnityEngine;
using UnityEngine.UIElements;

namespace GabUnity
{
    public class ProjectileManager : Manager_Base<ProjectileManager>
    {
        public static ProjectileController Shoot(Vector3 from, Vector3 dir, ProjectileController.ShootInfo shootInfo, string projectile_id = "")
        {
            var newobj = Instantiate(ProjectileDictionary.Get(projectile_id).gameObject, Instance.transform);
            var controller = newobj.GetComponent<ProjectileController>();

            newobj.transform.position = from;
            controller.Shoot(dir, shootInfo);

            return controller;
        }
    }
}