using System;
using UnityEngine;

namespace TW
{
    public abstract class BaseHealth : MonoBehaviour
    {
        [SerializeField]
        protected float maxHealth = 100f;

        protected float health;

        [SerializeField]
        Elements type;

        // current / max
        public event Action<float, float> HealthChanged;

        public Elements Type { get => type; }

        public float Health { get => health; }
        public float MaxHealth { get => maxHealth; }

        protected virtual void Start()
        {
            health = maxHealth;
            InvokeHealthChangedEvent();
        }

        protected void InvokeHealthChangedEvent() => HealthChanged?.Invoke(health, maxHealth);

        public virtual void TakeDamage(Elements damageType, float damage)
        {
            float damageMultiplier = DamageMultiplier.table[type][damageType];
            health -= damage * damageMultiplier;
            InvokeHealthChangedEvent();
        }
    }
}
