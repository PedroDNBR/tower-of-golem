using UnityEngine;

namespace TW
{
    public class KnightFollowPlayerState : IAIState
    {
        public void Enter(BaseAI baseAI)
        {
            baseAI.agent.stoppingDistance = baseAI.stoppingDistance;
            if (AICommander.Instance != null)
                AICommander.Instance.Register(baseAI as Knight);
    }

        public void Execute(BaseAI baseAI)
        {
            if (baseAI.currentPlayerInsight == null)
            {
                baseAI.SwitchState(States.roamingState);
                return;
            }

            float dist = Vector3.Distance(baseAI.transform.position, baseAI.currentPlayerInsight.transform.position);

            if (!baseAI.isBusy)
            {
                if (!baseAI.actionFlag)
                {
                    Vector3 dir = baseAI.currentPlayerInsight.transform.position - baseAI.transform.position;
                    dir.y = 0;
                    dir.Normalize();


                    float angle = Vector2.Angle(baseAI.transform.position, dir);
                    baseAI.currentSnapshot = baseAI.GetAction(dist, angle);

                    if(baseAI.currentSnapshot != null)
                    {
                        baseAI.SwitchState(KnightStates.attackPlayerState);
                        return;
                    }

                }
            }
             

            if (dist < baseAI.minAttackDistance)
            {
                baseAI.SwitchState(KnightStates.backingOffState);
                return;
            }

            baseAI.agent.SetDestination(baseAI.currentPlayerInsight.transform.position);
            baseAI.HandleRotation();
        }

        public void Exit(BaseAI baseAI) { }
    }
}