using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class KnightAvoidingHitState : IAIState
    {
        public void Enter(BaseAI baseAI)
        {
            Melee knight = baseAI as Melee;

            knight.agent.avoidancePriority = 50;

            BaseAI blocker = knight.GetBlockingEnemy();

            knight.circlePos = knight.GetOppositePosition(blocker);
            knight.agent.SetDestination(knight.circlePos);
        }

        public void Execute(BaseAI baseAI)
        {
            Melee knight = baseAI as Melee;

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
            Melee knight = baseAI as Melee;

            knight.circlePos = Vector3.zero;
        }
    }
}


