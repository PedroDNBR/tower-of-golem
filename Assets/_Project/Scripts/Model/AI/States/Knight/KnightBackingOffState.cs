using UnityEngine;
using UnityEngine.AI;

namespace TW
{
    public class KnightBackingOffState : IAIState
    {
        public void Enter(BaseAI baseAI)
        {
            Melee knight = baseAI as Melee;
            float backoffDistance = 2.5f;

            Vector3 toPlayer = knight.currentPlayerInsight.transform.position - knight.transform.position;
            Vector3 backDirection = -toPlayer.normalized;

            const int maxAttempts = 5;
            float angleStep = 45f;

            bool found = false;
            for (int i = 0; i < maxAttempts; i++)
            {
                float angleOffset = (i == 0) ? 0 : ((i % 2 == 0 ? 1 : -1) * angleStep * ((i + 1) / 2));
                Vector3 rotatedDir = Quaternion.Euler(0, angleOffset, 0) * backDirection;
                Vector3 targetPos = knight.transform.position + rotatedDir * backoffDistance;

                if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 5f, NavMesh.AllAreas))
                {
                    knight.backoffPos = hit.position;
                    knight.agent.SetDestination(knight.backoffPos);
                    found = true;
                    break;
                }
            }

            if (!found)
                knight.backoffPos = knight.transform.position;

            knight.backoffCooldown = 1.25f;
            knight.agent.avoidancePriority = 60;
        }

        public void Execute(BaseAI baseAI)
        {
            Melee knight = baseAI as Melee;

            knight.backoffCooldown -= Time.deltaTime;

            if (Vector3.Distance(knight.transform.position, knight.backoffPos) < 0.75f || knight.backoffCooldown <= 0)
                knight.SwitchState(knight.followPlayerState);

            knight.HandleRotation(true);
        }

        public void Exit(BaseAI baseAI)
        {
            Melee knight = baseAI as Melee;
            knight.backoffPos = Vector3.zero;
        }
    }
}