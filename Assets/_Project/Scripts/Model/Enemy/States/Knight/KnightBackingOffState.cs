using UnityEngine;
using UnityEngine.AI;

namespace TW
{
    public class KnightBackingOffState : IAIState
    {
        public void Enter(BaseAI baseAI)
        {
            Knight knight = baseAI as Knight;

            float backoffDistance = 2.5f;

            Vector3 toPlayer = knight.currentPlayerInsight.transform.position - knight.transform.position;
            Vector3 backDirection = -toPlayer.normalized;
            Vector3 targetPos = knight.transform.position + backDirection * backoffDistance;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 4f, NavMesh.AllAreas))
            {
                knight.backoffPos = hit.position;
                knight.agent.SetDestination(knight.backoffPos);
                knight.backoffCooldown = 1.25f;
                knight.agent.avoidancePriority = 60;
            }

            knight.agent.SetDestination(knight.backoffPos);
        }

        public void Execute(BaseAI baseAI)
        {
            Knight knight = baseAI as Knight;

            knight.backoffCooldown -= Time.deltaTime;

            if (Vector3.Distance(knight.transform.position, knight.backoffPos) < 0.75f || knight.backoffCooldown <= 0)
                knight.SwitchState(knight.followPlayerState);

            knight.HandleRotation();
        }

        public void Exit(BaseAI baseAI)
        {
            Knight knight = baseAI as Knight;
            knight.backoffPos = Vector3.zero;
        }
    }
}