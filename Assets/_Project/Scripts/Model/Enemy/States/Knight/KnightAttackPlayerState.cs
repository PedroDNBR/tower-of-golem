using UnityEngine;

namespace TW
{
    public class KnightAttackPlayerState : AttackPlayerState
    {
        public override void Execute(BaseAI baseAI)
        {
            if (baseAI.currentPlayerInsight == null)
            {
                baseAI.SwitchState(States.roamingState);
                return;
            }

            float distanceToPlayer = Vector3.Distance(baseAI.transform.position, baseAI.currentPlayerInsight.transform.position);
            if (distanceToPlayer < baseAI.minAttackDistance)
            {
                baseAI.SwitchState(KnightStates.backingOffState);
                return;
            }

            Knight knight = baseAI as Knight;

            if (knight.GetBlockingEnemy() != null)
            {
                knight.SwitchState(KnightStates.avoidingHitState);
                return;
            }

            if (!knight.isBusy)
            {
                if (!knight.actionFlag)
                {
                    if(baseAI.currentSnapshot == null)
                    {
                        //Vector3 dir = knight.currentPlayerInsight.transform.position - knight.transform.position;
                        //dir.y = 0;
                        //dir.Normalize();
                        //float angle = Vector2.Angle(knight.transform.position, dir);
                        //float dot = Vector3.Dot(knight.transform.right, dir);
                        //if (dot < 0) angle *= -1;
                        //var snapshot = knight.GetAction(distanceToPlayer, angle);
                        baseAI.SwitchState(KnightStates.followPlayerState);
                    }
                    if (baseAI.currentSnapshot != null)
                    {
                        knight.enemyController.AnimatorController.PlayTargetAnimation(baseAI.currentSnapshot.anim, true);
                        knight.recoveryTimer = baseAI.currentSnapshot.recoveryTime;
                        knight.actionFlag = true;
                    }
                }
            }
            knight.HandleRotation(true);
        }
    }
}