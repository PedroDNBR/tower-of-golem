using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TW
{
    public class AICommander : MonoBehaviour
    {
        public static AICommander Instance { get; private set; }

        public float checkInterval = 10f;
        public int maxAttackersPerPlayer = 4;

        public List<BaseAI> allEnemies = new();

        private Dictionary<PlayerController, List<BaseAI>> engagements = new();

        private void Awake()
        {
            if(!NetworkGameManager.Singleton.IsServer) Destroy(gameObject);
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            StartCoroutine(CoordinateCombatLoop());
        }

        public void Register(BaseAI knight)
        {
            if (!engagements.ContainsKey(knight.currentPlayerInsight))
                engagements[knight.currentPlayerInsight] = new List<BaseAI>();

            if (!engagements[knight.currentPlayerInsight].Contains(knight))
                engagements[knight.currentPlayerInsight].Add(knight);
        }

        public void Unregister(Knight knight)
        {
            if (engagements.ContainsKey(knight.currentPlayerInsight))
            {
                engagements[knight.currentPlayerInsight].Remove(knight);
                if (engagements[knight.currentPlayerInsight].Count == 0)
                    engagements.Remove(knight.currentPlayerInsight);
            }
        }

        private IEnumerator CoordinateCombatLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(checkInterval);
                Debug.Log(engagements.Count);
                foreach (var kvp in engagements)
                    CoordinateAttackersForPlayer(kvp.Key, kvp.Value);
            }
        }

        private void CoordinateAttackersForPlayer(PlayerController player, List<BaseAI> enemies)
        {
            Debug.Log(player);
            Debug.Log(enemies.Count);
            if (player == null || enemies.Count == 0) return;

            List<BaseAI> sorted = enemies.OrderBy(k => Vector3.Distance(k.transform.position, player.transform.position)).ToList();

            for (int i = 0; i < sorted.Count; i++)
            {
                var knight = sorted[i];
                if (knight == null) continue;

                if (i < maxAttackersPerPlayer)
                {
                    if (knight.debugtest) Debug.Log($"Force to switch to {knight.followPlayerState}", knight);
                    if (knight.CurrentState != knight.followPlayerState)
                        knight.SwitchState(knight.followPlayerState);
                }
                else
                {
                    if (knight.debugtest) Debug.Log($"Force to switch to {States.circleAroundPlayerState}", knight);
                    if(knight.CurrentState != States.circleAroundPlayerState) 
                        knight.SwitchState(States.circleAroundPlayerState);
                }
            }
        }
    }
}

