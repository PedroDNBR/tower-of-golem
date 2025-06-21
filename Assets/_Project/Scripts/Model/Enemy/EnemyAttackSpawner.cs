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

        private void Awake()
        {
            for (int i = 0; i < attackAttackDataList.Count; i++)
            {
                AttackInstanceData attackInstanceData = new AttackInstanceData();
                attackInstanceData.attackPrefab = attackAttackDataList[i].attackPrefab;
                attackInstanceData.origin = attackAttackDataList[i].origin;
                attacks.Add(attackAttackDataList[i].attackName, attackInstanceData);
            }
        }

        public void SpawnAttack(string name)
        {
            if (!IsServer) return;

            if (!IsServer)
            {
                Shoot(name); // visual imediato no client local
                ShootServerRpc(name); // envia pro server fazer o real
            }
            else
            {
                Shoot(name); // host também precisa ver o visual
                ShootClientRpc(name); // envia pra todos os outros clients
            }
        }

        private void Shoot(string name)
        {
            attacks[name].origin.gameObject.SetActive(true);
            GameObject damageOnEnterObj = Instantiate(attacks[name].attackPrefab, attacks[name].origin.position, attacks[name].origin.rotation);
            if (!IsServer) return;
            if (damageOnEnterObj.GetComponent<DealDamageWhenTriggerEnter>() != null)
                damageOnEnterObj.GetComponent<DealDamageWhenTriggerEnter>().CharacterBaseHealth = originHealth;
        }

        [ServerRpc]
        public void ShootServerRpc(string name)
        {
            Shoot(name); // servidor instancia o "real"
            ShootClientRpc(name); // replicar pros outros
        }

        [ClientRpc]
        public void ShootClientRpc(string name)
        {
            if (IsServer || IsLocalPlayer) return; // evita duplicar no host
            Shoot(name); // efeito visual nos outros clients
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
