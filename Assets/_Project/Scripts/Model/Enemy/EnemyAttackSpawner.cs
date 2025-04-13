using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class EnemyAttackSpawner : NetworkBehaviour
    {
        [SerializeField]
        private List<AttackList> attackLists = new List<AttackList>();

        [SerializeField]
        private Transform attackOrigin;

        private Transform attackTarget;

        private BaseHealth originHealth;

        public BaseHealth OriginHealth { set => originHealth = value; }

        private Dictionary<string, GameObject> attacks = new Dictionary<string, GameObject>();

        private void Awake()
        {
            for (int i = 0; i < attackLists.Count; i++)
                attacks.Add(attackLists[i].attackName, attackLists[i].attackPrefab);
        }

        public void SpawnAttack(string name)
        {
            if (!IsServer) return;
            if(attackTarget != null) attackOrigin.LookAt(attackTarget);

            if (!IsServer)
            {
                Shoot(attackOrigin.rotation, name); // visual imediato no client local
                ShootServerRpc(attackOrigin.rotation, name); // envia pro server fazer o real
            }
            else
            {
                Shoot(attackOrigin.rotation, name); // host também precisa ver o visual
                ShootClientRpc(attackOrigin.rotation, name); // envia pra todos os outros clients
            }
        }

        private void Shoot(Quaternion rotation, string name)
        {
            GameObject damageOnEnterObj = Instantiate(attacks[name], attackOrigin.position, rotation);
            if (!IsServer) return;
            if (damageOnEnterObj.GetComponent<DealDamageWhenTriggerEnter>() != null)
                damageOnEnterObj.GetComponent<DealDamageWhenTriggerEnter>().CharacterBaseHealth = originHealth;
        }

        [ServerRpc]
        public void ShootServerRpc(Quaternion rotation, string name)
        {
            Shoot(rotation, name); // servidor instancia o "real"
            ShootClientRpc(rotation, name); // replicar pros outros
        }

        [ClientRpc]
        public void ShootClientRpc(Quaternion rotation, string name)
        {
            if (IsServer || IsLocalPlayer) return; // evita duplicar no host
            Shoot(rotation, name); // efeito visual nos outros clients
        }

        public void SetPlayerAsAttackTarget(PlayerController player) => attackTarget = player.transform;
    }

    [Serializable]
    class AttackList
    {
        public string attackName;
        public GameObject attackPrefab;
    }
}
