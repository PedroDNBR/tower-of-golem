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

        private Dictionary<string, GameObject> attacks = new Dictionary<string, GameObject>();

        private void Awake()
        {
            for (int i = 0; i < attackLists.Count; i++)
                attacks.Add(attackLists[i].attackName, attackLists[i].attackPrefab);
        }

        public void SpawnAttack(string name)
        {
            Instantiate(attacks[name], attackOrigin.position, attackOrigin.rotation);
        }
    }

    [Serializable]
    class AttackList
    {
        public string attackName;
        public GameObject attackPrefab;
    }
}
