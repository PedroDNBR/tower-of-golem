using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class KnightAvoidingHitState : IAIState
    {
        public void Enter(BaseAI baseAI)
        {
            Knight knight = baseAI as Knight;

            knight.agent.avoidancePriority = 50;

            BaseAI blocker = knight.GetBlockingEnemy();

            knight.circlePos = knight.GetOppositePosition(blocker);
            knight.agent.SetDestination(knight.circlePos);
        }

        public void Execute(BaseAI baseAI)
        {
            Knight knight = baseAI as Knight;

            if (Vector3.Distance(knight.transform.position, knight.currentPlayerInsight.transform.position) < baseAI.minAttackDistance)
            {
                if (Random.Range(0, 9) < 4)
                {
                    baseAI.SwitchState(KnightStates.backingOffState);
                    return;
                }
            }

            if (Vector3.Distance(knight.transform.position, knight.circlePos) <= knight.stoppingDistance + .1f)
            {
                knight.SwitchState(knight.followPlayerState);
                return;
            }

            knight.HandleRotation(true);
        }

        public void Exit(BaseAI baseAI)
        {
            Knight knight = baseAI as Knight;

            knight.circlePos = Vector3.zero;
        }
    }
}


