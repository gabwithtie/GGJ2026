using System.Collections.Generic;
using UnityEngine;

namespace GabUnity
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider), typeof(AudioSource))]
    public class ProjectileController : MonoBehaviour
    {
        [System.Serializable]
        public struct ShootInfo
        {
            [SerializeField] private UnitIdentifier from;
            public UnitIdentifier From => from;

            public float projectile_speed;
            public float max_pierce_count;
            public float max_bounce_count;
            public float damage;

            public ShootInfo(UnitIdentifier _from)
            {
                from = _from;
                projectile_speed = 10f;
                max_pierce_count = 0;
                max_bounce_count = 0;
                damage = 1f;
            }
        }

        private Rigidbody mRigidbody;
        private SphereCollider mSphereCollider;
        private AudioSource mAudioSource;
        private TrailRenderer optional_trail;

        [Header("Vfx")]
        [SerializeField] private string ondeath_globaleffect_id;
        [SerializeField] private string onhit_globaleffect_id;
        [SerializeField] private string onbounce_globaleffect_id;
        [Header("Audio")]
        [SerializeField] private string onbounce_audio_id;

        [Header("Projectile Info")]
        [SerializeField] private ShootInfo shoot_info;



        public readonly Queue<Collider> ignore_list = new();

        public UnitIdentifier From => shoot_info.From;
        private int bounce_count = 0;
        private int pierce_count = 0;

        private Vector3 late_velocity;


        private void Awake()
        {
            mRigidbody = GetComponent<Rigidbody>();
            mSphereCollider = GetComponent<SphereCollider>();
            optional_trail = GetComponent<TrailRenderer>();
            mAudioSource = GetComponent<AudioSource>();
        }
        private void IgnoreCollider(Collider other)
        {
            ignore_list.Enqueue(other);
            Physics.IgnoreCollision(mSphereCollider, other);
        }

        private void ResetIgnores()
        {
            while (ignore_list.TryDequeue(out var ignoredCollider))
            {
                if (ignoredCollider != null)
                {
                    Physics.IgnoreCollision(mSphereCollider, ignoredCollider, false);
                }
            }

            if (From != null)
                IgnoreCollider(From.Collider);
        }

        public void Shoot(Vector3 dir, ShootInfo _shootinfo, ProjectileController parent = null)
        {
            this.shoot_info = _shootinfo;
            ResetIgnores();

            if (optional_trail)
                optional_trail.Clear();

            mRigidbody.linearVelocity = dir * shoot_info.projectile_speed;

            bounce_count = 0;
            pierce_count = 0;

            if (parent == null)
                return;

            bounce_count = parent.bounce_count;
            pierce_count = parent.pierce_count;
        }

        private void LateUpdate()
        {
            late_velocity = mRigidbody.linearVelocity;
        }

        private void OnCollisionEnter(Collision collision)
        {
            void IgnoreThisCollision()
            {
                IgnoreCollider(collision.collider);
                mRigidbody.linearVelocity = late_velocity;
            }

            var objectToCheck = collision.collider.gameObject;

            if (collision.rigidbody != null)
            {
                objectToCheck = collision.rigidbody.gameObject;
                collision.rigidbody.AddExplosionForce(shoot_info.projectile_speed * 2, this.transform.position, mSphereCollider.radius + 3);
            }

            if (objectToCheck.TryGetComponent(out UnitIdentifier hittable))
            {
                if (From.TeamId == hittable.TeamId)
                {
                    IgnoreThisCollision();
                    return;
                }

                if(hittable.TryGetComponent(out HealthObject healthObject))
                {
                    healthObject.TakeDamage(shoot_info.damage, From);
                }

                if (pierce_count < shoot_info.max_pierce_count)
                {
                    IgnoreThisCollision();
                }

                pierce_count++;

                if (onhit_globaleffect_id != "")
                    GlobalEffectManager.Spawn(onhit_globaleffect_id, transform.position, collision.GetContact(0).normal);
            }
            else
            {
                bounce_count++;

                mAudioSource.PlayOneShotSafe(AudioDictionary.Get("onbounce_audio_id"));

                ResetIgnores();
                if (onbounce_globaleffect_id != "")
                    GlobalEffectManager.Spawn(onbounce_globaleffect_id, transform.position, collision.GetContact(0).normal);
            }

            if (pierce_count > shoot_info.max_pierce_count)
                KillProjectile(hittable);
            else if (bounce_count > shoot_info.max_bounce_count)
                KillProjectile(hittable);
        }

        private void KillProjectile(UnitIdentifier hit = null)
        {
            if(ondeath_globaleffect_id != "")
                GlobalEffectManager.Spawn(ondeath_globaleffect_id, transform.position, late_velocity);

            Destroy(this.gameObject);
        }
    }
}