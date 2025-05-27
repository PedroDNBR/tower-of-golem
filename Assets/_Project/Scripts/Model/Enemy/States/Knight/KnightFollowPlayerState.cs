using UnityEngine;

namespace TW
{
    public class KnightFollowPlayerState : IAIState
    {
        public void Enter(BaseAI baseAI)
        {
            baseAI.agent.stoppingDistance = baseAI.stoppingDistance;
        }

        public void Execute(BaseAI baseAI)
        {
            if (baseAI.currentPlayerInsight == null)
            {
                baseAI.SwitchState(States.roamingState);
                return;
            }

            float dist = Vector3.Distance(baseAI.transform.position, baseAI.currentPlayerInsight.transform.position);

            if (dist < 1f)
            {
                baseAI.SwitchState(KnightStates.backingOffState);
                return;
            }

            if (dist < baseAI.maxAttackDistance)
            {
                baseAI.SwitchState(KnightStates.attackPlayerState);
                return;
            }

            baseAI.agent.SetDestination(baseAI.currentPlayerInsight.transform.position);
            baseAI.HandleRotation();
        }

        public void Exit(BaseAI baseAI) { }
    }
}