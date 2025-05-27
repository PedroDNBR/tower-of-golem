using UnityEngine;

namespace TW
{
    public class KnightAttackPlayerState : IAIState
    {
        public void Enter(BaseAI baseAI)
        {
            baseAI.agent.avoidancePriority = 10;
        }

        public void Execute(BaseAI baseAI)
        {
            if (baseAI.currentPlayerInsight == null)
            {
                baseAI.SwitchState(States.roamingState);
                return;
            }

            float distanceToPlayer = Vector3.Distance(baseAI.transform.position, baseAI.currentPlayerInsight.transform.position);
            if (distanceToPlayer < 1f)
            {
                baseAI.SwitchState(KnightStates.backingOffState);
                return;
            }

            Knight knight = baseAI as Knight;

            if (!knight.isBusy)
            {
                if (knight.actionFlag)
                {
                    knight.recoveryTimer -= Time.deltaTime;
                    if (knight.recoveryTimer <= 0)
                    {
                        baseAI.actionFlag = false;
                        knight.SwitchState(States.followPlayerState);
                        return;
                    }
                }
                else
                {
                    if(knight.GetBlockingEnemy() != null)
                    {
                        knight.SwitchState(KnightStates.avoidingHitState);
                        return;
                    }

                    Vector3 dir = knight.currentPlayerInsight.transform.position - knight.transform.position;
                    dir.y = 0;
                    dir.Normalize();

                    float angle = Vector2.Angle(knight.transform.position, dir);
                    float dot = Vector3.Dot(knight.transform.right, dir);
                    if (dot < 0) angle *= -1;

                    var snapshot = knight.GetAction(distanceToPlayer, angle);
                    if (snapshot != null)
                    {
                        knight.enemyController.AnimatorController.PlayTargetAnimation(snapshot.anim, true);
                        knight.recoveryTimer = snapshot.recoveryTime;
                        knight.actionFlag = true;
                    }
                }
            }

            knight.HandleRotation(true);
        }

        public void Exit(BaseAI baseAI) { }
    }
}