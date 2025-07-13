using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public abstract class BaseHealth : NetworkBehaviour
    {
        [SerializeField]
        public NetworkVariable<float> maxHealth = new NetworkVariable<float>(100f);

        public NetworkVariable<float> health = new NetworkVariable<float>(0);

        [SerializeField]
        protected Elements type;

        // current / max
        public event Action<float, float> HealthChanged;

        public Elements Type { get => type; }

        public float Health { get => health.Value; }
        public float MaxHealth { get => maxHealth.Value; }

        public event Action Dead;

        protected List<GameObject> objectsThatDamaged = new List<GameObject>();

        protected virtual void Start()
        {
            InitOnHealthChangedAction();
            if (IsServer) health.Value = maxHealth.Value;
        }

        public void InitOnHealthChangedAction()
        {
            health.OnValueChanged += (float old, float current) => HealthChanged?.Invoke(health.Value, maxHealth.Value);
        }

        public virtual void TakeDamage(Elements damageType, float damage, GameObject origin)
        {
            if (!IsServer) return;
            if (objectsThatDamaged.Contains(origin)) return;

            objectsThatDamaged.Add(origin);

            float damageMultiplier = DamageMultiplier.table[type][damageType];
            health.Value -= damage * damageMultiplier;

            if (health.Value <= 0) InvokeDead();
        }

        private void LateUpdate()
        {
            if(!IsServer) return;
            if(objectsThatDamaged.Count > 0) objectsThatDamaged.Clear();
        }

        public void InvokeDead()
        {
            Dead?.Invoke();
            Dead = null;
        }

        public void InvokeHealthUpdateCallback() => HealthChanged?.Invoke(health.Value, maxHealth.Value);
    }
}
