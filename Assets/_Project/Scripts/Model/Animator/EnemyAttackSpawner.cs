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
                attackInstanceData.attackId = attackAttackDataList[i].attackId;
                attackInstanceData.origin = attackAttackDataList[i].origin;
                attacks.Add(attackAttackDataList[i].attackName, attackInstanceData);
            }

            projectileTracker = GetComponent<ProjectileTracker>();
        }

        public void SpawnAttack(string name)
        {
            if (!IsServer) return;

            attacks[name].origin.gameObject.SetActive(true);
            ShootServerRpc(name, attacks[name].origin.position, attacks[name].origin.rotation);
        }

        private void Shoot(string name, Vector3 position, Quaternion rotation)
        {
            attacks[name].origin.gameObject.SetActive(true);
            PoolObject damageOnEnterObj = ObjectPoolController.instance.InstantiateSpell(attacks[name].attackId, position, rotation);
            if (damageOnEnterObj.GetComponent<AddToTrackedProjectiles>() != null)
                if (projectileTracker != null) damageOnEnterObj.GetComponent<AddToTrackedProjectiles>().AddToTracker(projectileTracker, attacks[name].attackId);
            
            if (!IsServer) return;

            DealDamageWhenTrigger dealDamageWhenTrigger = damageOnEnterObj.GetComponent<DealDamageWhenTrigger>();
            if (dealDamageWhenTrigger == null) dealDamageWhenTrigger = damageOnEnterObj.GetComponentInChildren<DealDamageWhenTrigger>();
            if (dealDamageWhenTrigger == null) dealDamageWhenTrigger = damageOnEnterObj.GetComponentInParent<DealDamageWhenTrigger>();
            if (dealDamageWhenTrigger != null) dealDamageWhenTrigger.CharacterBaseHealth = originHealth;

            StopProjectileWhenCollide stopProjectileWhenCollide = damageOnEnterObj.GetComponent<StopProjectileWhenCollide>();
            if (stopProjectileWhenCollide == null) stopProjectileWhenCollide = damageOnEnterObj.GetComponentInChildren<StopProjectileWhenCollide>();
            if (stopProjectileWhenCollide == null) stopProjectileWhenCollide = damageOnEnterObj.GetComponentInParent<StopProjectileWhenCollide>();
            if (stopProjectileWhenCollide != null) stopProjectileWhenCollide.origin = transform.root;

        }

        [ServerRpc]
        public void ShootServerRpc(string name, Vector3 position, Quaternion rotation)
        {
            Shoot(name, position, rotation);
            ShootClientRpc(name, position, rotation);
        }

        [ClientRpc]
        public void ShootClientRpc(string name, Vector3 position, Quaternion rotation)
        {
            if (IsServer || IsLocalPlayer) return;
            attacks[name].origin.gameObject.SetActive(true);
            PoolObject damageOnEnterObj = ObjectPoolController.instance.InstantiateSpell(attacks[name].attackId, position, rotation);
            if (damageOnEnterObj.GetComponent<AddToTrackedProjectiles>() != null)
                if (projectileTracker != null) damageOnEnterObj.GetComponent<AddToTrackedProjectiles>().AddToTracker(projectileTracker, attacks[name].attackId);
            
            StopProjectileWhenCollide stopProjectileWhenCollide = damageOnEnterObj.GetComponent<StopProjectileWhenCollide>();
            if (stopProjectileWhenCollide == null) stopProjectileWhenCollide = damageOnEnterObj.GetComponentInChildren<StopProjectileWhenCollide>();
            if (stopProjectileWhenCollide == null) stopProjectileWhenCollide = damageOnEnterObj.GetComponentInParent<StopProjectileWhenCollide>();
            if (stopProjectileWhenCollide != null) stopProjectileWhenCollide.origin = transform.root;
        }
    }

    [Serializable]
    class AttackData
    {
        public string attackName;
        public string attackId;
        public Transform origin;
    }

    [Serializable]
    class AttackInstanceData
    {
        public string attackId;
        public Transform origin;
    }
}
