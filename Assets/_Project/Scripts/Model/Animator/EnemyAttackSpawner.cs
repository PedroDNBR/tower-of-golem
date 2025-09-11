using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class EnemyAttackSpawner : NetworkBehaviour
    {
        [SerializeField]
        private List<AttackData> attackAttackDataList = new List<AttackData>();

        private BaseHealth originHealth;

        public BaseHealth OriginHealth { set => originHealth = value; }

        private Dictionary<string, AttackInstanceData> attacks = new Dictionary<string, AttackInstanceData>();

        private ProjectileTracker projectileTracker;

        private void Awake()
        {
            for (int i = 0; i < attackAttackDataList.Count; i++)
            {
                AttackInstanceData attackInstanceData = new AttackInstanceData();
                attackInstanceData.attackPrefab = attackAttackDataList[i].attackPrefab;
                attackInstanceData.origin = attackAttackDataList[i].origin;
                attacks.Add(attackAttackDataList[i].attackName, attackInstanceData);
            }

            projectileTracker = GetComponent<ProjectileTracker>();
        }

        public void SpawnAttack(string name)
        {
            if (!IsServer) return;

            ShootServerRpc(name, attacks[name].origin.position, attacks[name].origin.rotation);
        }

        private void Shoot(string name)
        {
            attacks[name].origin.gameObject.SetActive(true);
            GameObject damageOnEnterObj = Instantiate(attacks[name].attackPrefab, attacks[name].origin.position, attacks[name].origin.rotation);
            if (damageOnEnterObj.GetComponent<AddToTrackedProjectiles>() != null)
                if (projectileTracker != null) damageOnEnterObj.GetComponent<AddToTrackedProjectiles>().AddToTracker(projectileTracker);
            if (!IsServer) return;
            if (damageOnEnterObj.GetComponent<DealDamageWhenTriggerEnter>() != null)
                damageOnEnterObj.GetComponent<DealDamageWhenTriggerEnter>().CharacterBaseHealth = originHealth;
        }

        [ServerRpc]
        public void ShootServerRpc(string name, Vector3 position, Quaternion rotation)
        {
            Shoot(name);
            ShootClientRpc(name, position, rotation);
        }

        [ClientRpc]
        public void ShootClientRpc(string name, Vector3 position, Quaternion rotation)
        {
            if (IsServer || IsLocalPlayer) return;
            attacks[name].origin.gameObject.SetActive(true);
            Instantiate(attacks[name].attackPrefab, position, rotation);
        }
    }

    [Serializable]
    class AttackData
    {
        public string attackName;
        public GameObject attackPrefab;
        public Transform origin;
    }

    [Serializable]
    class AttackInstanceData
    {
        public GameObject attackPrefab;
        public Transform origin;
    }
}
