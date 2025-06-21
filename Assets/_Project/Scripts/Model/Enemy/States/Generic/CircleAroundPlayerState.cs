using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace TW
{
    public class CircleAroundPlayerState : IAIState
    {
        Vector3 relativeOffset;

        float time = 0;

        public void Enter(BaseAI baseAI)
        {
            baseAI.agent.stoppingDistance = baseAI.stoppingDistance;
            Vector3 relativeOffset = baseAI.GetLocationAroundPosition(7f, 8f);
            Vector3 targetPos = baseAI.currentPlayerInsight.transform.position + relativeOffset;
            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                baseAI.circlePos = hit.position;
                baseAI.agent.SetDestination(hit.position);
            }
        }

        public void Execute(BaseAI baseAI)
        {
            if (baseAI.currentPlayerInsight == null)
            {
                baseAI.SwitchState(States.roamingState);
                return;
            }

            if(time > 1.5f)
            {
                FollowNewPosition(baseAI);
                time = 0;
            }

            time += Time.deltaTime;

            // Calcula o ponto de destino relativo à posição atual do player
            Vector3 targetPos = baseAI.currentPlayerInsight.transform.position + relativeOffset;
            float distToTarget = Vector3.Distance(baseAI.transform.position, targetPos);

            // Chegou perto da posição? Gera nova posição circular
            if (distToTarget < baseAI.stoppingDistance + 0.4f)
            {
                UpdateCircleTarget(baseAI);
            }

            baseAI.HandleRotation();
        }

        public void Exit(BaseAI baseAI) { }

        private void UpdateCircleTarget(BaseAI baseAI)
        {
            // Novo ângulo e raio
            relativeOffset = baseAI.GetLocationAroundPosition(14f, 16f);
        }

        private void FollowNewPosition(BaseAI baseAI)
        {
            Vector3 targetPos = baseAI.currentPlayerInsight.transform.position + relativeOffset;

            if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                baseAI.circlePos = hit.position;
                baseAI.agent.SetDestination(hit.position);
                if (baseAI.debugtest) Debug.Log($"[UPDATE] New circle pos: {hit.position}", baseAI);
            }
        }
    }
}