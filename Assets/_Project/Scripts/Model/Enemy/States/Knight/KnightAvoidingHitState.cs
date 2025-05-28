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
            if (knight.circlePos == Vector3.zero)
                knight.circlePos = knight.GetOppositePosition(blocker);
        }

        public void Execute(BaseAI baseAI)
        {
            Knight knight = baseAI as Knight;
            BaseAI blocker = knight.GetBlockingEnemy();

            if (Vector3.Distance(knight.transform.position, knight.circlePos) <= 1.3f || blocker == null)
            {
                knight.SwitchState(knight.followPlayerState);
                return;
            }

            if (knight.agent.destination != knight.circlePos)
                knight.agent.SetDestination(knight.circlePos);

            knight.HandleRotation();
        }

        public void Exit(BaseAI baseAI)
        {
            Knight knight = baseAI as Knight;

            knight.circlePos = Vector3.zero;
            knight.agent.SetDestination(knight.currentPlayerInsight.transform.position);
        }
    }
}


