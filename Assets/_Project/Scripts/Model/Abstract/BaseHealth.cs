using System;
using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public abstract class BaseHealth : MonoBehaviour
    {
        [SerializeField]
        protected float maxHealth = 100f;

        protected float health;

        [SerializeField]
        protected Elements type;

        // current / max
        public event Action<float, float> HealthChanged;

        public Elements Type { get => type; }

        public float Health { get => health; }
        public float MaxHealth { get => maxHealth; }

        public event Action Dead;

        protected List<GameObject> objectsThatDamaged = new List<GameObject>();

        protected virtual void Start()
        {
            health = maxHealth;
            InvokeHealthChangedEvent();
        }

        protected void InvokeHealthChangedEvent() => HealthChanged?.Invoke(health, maxHealth);

        public virtual void TakeDamage(Elements damageType, float damage, GameObject origin)
        {
            if (objectsThatDamaged.Contains(origin)) return;

            objectsThatDamaged.Add(origin);

            float damageMultiplier = DamageMultiplier.table[type][damageType];
            health -= damage * damageMultiplier;
            InvokeHealthChangedEvent();

            if (health <= 0) InvokeDead();
        }

        private void LateUpdate()
        {
            if(objectsThatDamaged.Count > 0) objectsThatDamaged.Clear();
        }

        public void InvokeDead() => Dead?.Invoke();  
    }
}
