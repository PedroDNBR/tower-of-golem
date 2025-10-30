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

            float distanceToPlayer = Vector3.Distance(baseAI.transform.position, baseAI.currentPlayerInsight.transform.position);

            if (!baseAI.enemyController.AnimatorController.GetIsBusyBool())
            {
                if (baseAI.currentSnapshot == null)
                {
                    //Vector3 dir = baseAI.currentPlayerInsight.transform.position - baseAI.transform.position;
                    //dir.y = 0;
                    //dir.Normalize();
                    //float angle = Vector2.Angle(baseAI.transform.position, dir);
                    //float dot = Vector3.Dot(baseAI.transform.right, dir);
                    //if (dot < 0) angle *= -1;
                    //var snapshot = baseAI.GetAction(distanceToPlayer, angle);
                    baseAI.SwitchState(baseAI.followPlayerState);
                }
                if (!baseAI.actionFlag && baseAI.recoveryTimer <= 0)
                {
                    if (baseAI.currentSnapshot != null)
                    {
                        baseAI.enemyController.AnimatorController.PlayTargetAnimation(baseAI.currentSnapshot.anim, true);
                        baseAI.recoveryTimer = baseAI.currentSnapshot.recoveryTime;
                        baseAI.actionFlag = true;
                        baseAI.currentSnapshot = null;
                    }
                }
            }
            baseAI.HandleRotation(true);
        }

        public virtual void Exit(BaseAI baseAI) { }
    }
}