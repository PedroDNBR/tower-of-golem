using UnityEngine;
using UnityEngine.AI;

namespace TW
{
    public class RunAwayFromPlayerState : IAIState
    {
        Vector3 relativeOffset;
        float cachedSpeed;

        public void Enter(BaseAI baseAI)
        {
            if(baseAI.currentPlayerInsight != null)
            {
                Vector3 direction = (baseAI.transform.position - baseAI.currentPlayerInsight.transform.position).normalized;
                float randomDistance = Random.Range(8f, 12f);
                Vector3 playerOppositeDireciton = direction * randomDistance;
                Debug.Log(direction);
                Debug.Log(randomDistance);
                Debug.Log(playerOppositeDireciton);
                relativeOffset = playerOppositeDireciton + baseAI.GetLocationAroundPosition(0, 3f);
                Debug.Log("run away from player");
            }
            else
            {
                relativeOffset = baseAI.transform.position + baseAI.GetLocationAroundPosition(10f, 14f);
                Debug.Log("run away");
            }
            Debug.Log(relativeOffset);
            if (NavMesh.SamplePosition(relativeOffset, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                baseAI.circlePos = hit.position;
                baseAI.agent.SetDestination(hit.position);
                cachedSpeed = baseAI.walkSpeed;
                baseAI.walkSpeed *= 2.5f;
                relativeOffset = hit.position;
            }
            else
            {
                // Handle cases where no valid position is found (e.g., fallback behavior)
                Debug.LogWarning("No valid position found!");
            }

            Debug.Log(relativeOffset);
        }

        public void Execute(BaseAI baseAI)
        {
            float distToTarget = Vector3.Distance(baseAI.transform.position, baseAI.agent.destination);
            if (distToTarget < baseAI.stoppingDistance + 0.5f)
            {
                baseAI.SwitchState(States.roamingState);
            }

            baseAI.HandleRotation();
        }

        public void Exit(BaseAI baseAI)
        {
            relativeOffset = Vector3.zero;
            baseAI.currentPlayerInsight = null;
            baseAI.walkSpeed = cachedSpeed;
        }
    }
}

