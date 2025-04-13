using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class EnemyAttackEnableColliders : MonoBehaviour
    {
        [SerializeField]
        private List<DamageCollider> colliders = new List<DamageCollider>();

        Dictionary<string, Collider> damageColliders = new Dictionary<string, Collider>();

        private void Start()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            for (int i = 0; i < colliders.Count; i++)
                damageColliders.Add(colliders[i].colliderName, colliders[i].collider);
        }

        public void EnableDamageColliders(string name) => SetDamageColliders(name, true);

        public void DisableDamageColliders(string name) => SetDamageColliders(name, false);

        private void SetDamageColliders(string name, bool isEnabled)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            string[] colliderNames = name.Split(",");
            for (int i = 0; i < colliderNames.Length; i++)
                damageColliders[colliderNames[i]].enabled = isEnabled;
        }
    }

    [Serializable]
    class DamageCollider
    {
        public string colliderName;
        public Collider collider;
    }
}