using UnityEngine;

namespace TW
{
    public class AttackPlayerState : IAIState
    {
        public virtual void Enter(BaseAI baseAI)
        {
            baseAI.agent.avoidancePriority = 10;
        }

        public virtual void Execute(BaseAI baseAI)
        {
            if (baseAI.currentPlayerInsight == null)
            {
                baseAI.SwitchState(States.roamingState);
                return;
            }

            if (!baseAI.isBusy)
            {
                if (baseAI.actionFlag)
                {
                    baseAI.recoveryTimer -= Time.deltaTime;
                    if (baseAI.recoveryTimer <= 0)
                    {
                        baseAI.actionFlag = false;
                        baseAI.SwitchState(baseAI.followPlayerState);
                        return;
                    }
                    else
                    {
                        baseAI.agent.SetDestination(baseAI.currentPlayerInsight.transform.position);
                    }
                }
                else
                {
                    Vector3 dir = baseAI.currentPlayerInsight.transform.position - baseAI.transform.position;
                    dir.y = 0;
                    dir.Normalize();

                    float angle = Vector2.Angle(baseAI.transform.position, dir);
                    float dot = Vector3.Dot(baseAI.transform.right, dir);
                    if (dot < 0) angle *= -1;

                    float distanceToPlayer = Vector3.Distance(baseAI.transform.position, baseAI.currentPlayerInsight.transform.position);
                    
                    var snapshot = baseAI.GetAction(distanceToPlayer, angle);
                    if (snapshot != null)
                    {
                        baseAI.enemyController.AnimatorController.PlayTargetAnimation(snapshot.anim, true);
                        baseAI.recoveryTimer = snapshot.recoveryTime;
                        baseAI.actionFlag = true;
                    }
                }
            }

            if (baseAI.enemyController.AnimatorController.GetCanRotate()) baseAI.HandleRotation(true);
        }

        public virtual void Exit(BaseAI baseAI) { }
    }
}