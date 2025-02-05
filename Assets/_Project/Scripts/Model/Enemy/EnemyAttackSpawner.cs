using System;
using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class EnemyAttackSpawner : MonoBehaviour
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
            if(attackTarget != null) attackOrigin.LookAt(attackTarget);

            GameObject damageOnEnterObj = Instantiate(attacks[name], attackOrigin.position, attackOrigin.rotation);

            if (damageOnEnterObj.GetComponent<DealDamageWhenTriggerEnter>() != null)
                damageOnEnterObj.GetComponent<DealDamageWhenTriggerEnter>().CharacterBaseHealth = originHealth;
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
