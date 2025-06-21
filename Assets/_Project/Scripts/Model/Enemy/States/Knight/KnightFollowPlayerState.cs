using UnityEngine;

namespace TW
{
    public class KnightFollowPlayerState : IAIState
    {
        public void Enter(BaseAI baseAI)
        {
            baseAI.enemyController.AnimatorController.PlayTargetAnimation("DrawSword", true);
            baseAI.agent.stoppingDistance = baseAI.stoppingDistance;
            if (AICommander.Instance != null)
                AICommander.Instance.Register(baseAI as Knight, baseAI.currentPlayerInsight);
    }

        public void Execute(BaseAI baseAI)
        {
            if (baseAI.currentPlayerInsight == null)
            {
                baseAI.SwitchState(States.roamingState);
                return;
            }

            float dist = Vector3.Distance(baseAI.transform.position, baseAI.currentPlayerInsight.transform.position);

            if (dist < baseAI.minAttackDistance)
            {
                baseAI.SwitchState(KnightStates.backingOffState);
                return;
            }

            if (dist <= baseAI.maxAttackDistance)
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