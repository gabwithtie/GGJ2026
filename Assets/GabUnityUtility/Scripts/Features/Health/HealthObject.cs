using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GabUnity
{
    [RequireComponent(typeof(UnitIdentifier))]
    public class HealthObject : MonoBehaviour
    {
        [SerializeField] private float health;
        [SerializeField] private float maxhealth;
        public float Health => health;
        public float MaxHealth => maxhealth;

        [SerializeField] private bool DestroyOnDie = true;

        [SerializeField] private UnityEvent<float, UnitIdentifier> onDamageEvent;
        private Action<UnitIdentifier> onDie;
        private Action<float, UnitIdentifier> onDamage;
        public void SubscribeOnDie(Action<UnitIdentifier> newaction)
        {
            onDie += newaction;
        }
        public void SubscribeOnDamage(Action<float, UnitIdentifier> newaction)
        {
            onDamage += newaction;
        }

        public void TakeDamage(float damage, UnitIdentifier source)
        {
            if (health <= 0)
                return;

            health -= damage;

            onDamageEvent.Invoke(damage, source);
            onDamage?.Invoke(damage, source);

            if (health > 0)
                return;

            onDie?.Invoke(source);
            if (DestroyOnDie)
            {
                Destroy(this.gameObject);
            }
        }
    }
}