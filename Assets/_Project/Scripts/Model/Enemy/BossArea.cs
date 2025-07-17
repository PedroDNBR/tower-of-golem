using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class BossArea : MonoBehaviour
    {
        [SerializeField]
        public EnemyController boss;

        [SerializeField]
        private GameObject bossPrefab;
        [SerializeField]
        private Transform spawnPoint;

        private List<PlayerController> playersInArea = new List<PlayerController>();

        public static BossArea instance;

        public BossArea() { instance = this; }

        public Action BossSpawned;

        public Vector3 startScale;

        private void OnEnable()
        {
            enabled = NetworkGameManager.Singleton.IsServer;
        }

        private void Start()
        {
            startScale = transform.localScale;

            transform.localScale = transform.localScale * .7f;
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            if (player == null) return;

            if(!playersInArea.Contains(player))
                playersInArea.Add(player);

            if (playersInArea.Count == ((NetworkGameManager)NetworkManager.Singleton).ConnectedClientsIds.Count)
            {
                Debug.Log((NetworkGameManager)NetworkManager.Singleton);
                if(((NetworkGameManager)NetworkManager.Singleton).IsServer && boss == null)
                {
                    GameObject spawnedBoss = ((NetworkGameManager)NetworkManager.Singleton).SpawnMob(bossPrefab, spawnPoint);
                    boss = spawnedBoss.GetComponent<EnemyController>();
                    boss.gameObject.SetActive(true);
                    boss.enabled = true;
                    boss.BaseAI.enabled = true;
                    BossSpawned.Invoke();
                }
            } 
            if (boss != null && boss.gameObject.activeSelf)
            {
                player.PlayerUI.SetBossUIInPlayerVisible(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (boss == null) return;

            PlayerController player = other.gameObject.GetComponentInParent<PlayerController>();
            if (player == null) return;

            if (!playersInArea.Contains(player))
                playersInArea.Remove(player);

            player.PlayerUI.SetBossUIInPlayerVisible(false);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < playersInArea.Count; i++)
            {
                playersInArea[i].PlayerUI.SetBossUIInPlayerVisible( false);
            }
            if(boss != null) boss.UnsetHealthListener();
        }
    }
}
