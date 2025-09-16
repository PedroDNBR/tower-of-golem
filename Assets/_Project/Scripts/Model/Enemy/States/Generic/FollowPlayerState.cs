using UnityEngine;

namespace TW
{
    public class FollowPlayerState : IAIState
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

            if (!baseAI.enemyController.AnimatorController.GetIsBusyBool())
            {
                if (!baseAI.actionFlag)
                {
                    Vector3 dir = baseAI.currentPlayerInsight.transform.position - baseAI.transform.position;
                    dir.y = 0;
                    dir.Normalize();

                    float angle = Vector2.Angle(baseAI.transform.forward, dir);
                    baseAI.currentSnapshot = baseAI.GetAction(dist, angle);
                    
                    if (baseAI.currentSnapshot != null)
                    {
                        baseAI.SwitchState(States.attackPlayerState);
                        return;
                    }

                }
                if (baseAI.actionFlag && baseAI.recoveryTimer > 0)
                {
                    baseAI.recoveryTimer -= Time.deltaTime;
                    if (baseAI.recoveryTimer <= 0)
                    {
                        baseAI.actionFlag = false;
                    }
                }
            }
            baseAI.agent.SetDestination(baseAI.currentPlayerInsight.transform.position);
            baseAI.HandleRotation();
        }

        public void Exit(BaseAI baseAI) { }
    }
}