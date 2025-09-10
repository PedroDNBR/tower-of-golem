using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class AICommander : MonoBehaviour
    {
        public static AICommander Instance { get; private set; }

        public float checkInterval = 10f;
        public int maxAttackersPerPlayer = 4;

        private Dictionary<PlayerController, List<BaseAI>> engagements = new();

        private readonly List<BaseAI> activeEnemies = new();

        private void Awake()
        {
            if(!NetworkManager.Singleton.IsServer) Destroy(gameObject);

            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            StartCoroutine(CoordinateCombatLoop());
        }

        public void Register(BaseAI ai)
        {
            if (!activeEnemies.Contains(ai))
                activeEnemies.Add(ai);
        }

        public void Unregister(BaseAI ai)
        {
            if (activeEnemies.Contains(ai))
                activeEnemies.Remove(ai);
        }

        private IEnumerator CoordinateCombatLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(checkInterval);

                //Debug.Log(engagements.Count);
                CoordinateAll();
            }
        }

        private void CoordinateAll()
        {
            foreach (var ai in activeEnemies)
            {
                if (ai == null) continue;


                var bestTarget = ai.GetBestTarget();
                if (bestTarget != null)
                    ai.currentPlayerInsight = bestTarget;
            }

            var groups = activeEnemies
                .Where(ai => ai != null && ai.currentPlayerInsight != null)
                .GroupBy(ai => ai.currentPlayerInsight);

            foreach (var group in groups)
            {
                CoordinateAttackersForPlayer(group.Key, group.ToList());
            }
        }

        private void CoordinateAttackersForPlayer(PlayerController player, List<BaseAI> enemies)
        {
            if (player == null || enemies.Count == 0) return;

            List<BaseAI> sorted = enemies
                .OrderBy(k => Vector3.Distance(k.transform.position, player.transform.position))
                .ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                var knight = sorted[i];
                if (knight == null) continue;

                if (i < maxAttackersPerPlayer)
                {
                    if (knight.CurrentState != knight.followPlayerState)
                        knight.SwitchState(knight.followPlayerState);
                }
                else
                {
                    if (knight.CurrentState != States.circleAroundPlayerState)
                        knight.SwitchState(States.circleAroundPlayerState);
                }
            }
        }
    }
}

