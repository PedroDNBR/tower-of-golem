using UnityEngine;

namespace TW
{
    public class RoamingState : IAIState
    {
        public void Enter(BaseAI baseAI)
        {
            baseAI.finalDestination = baseAI.FindRoamingSpot();
            baseAI.agent.SetDestination(baseAI.finalDestination);
        }

        public virtual void Execute(BaseAI baseAI)
        {
            if (baseAI.currentPlayerInsight != null)
            {
                baseAI.SwitchState(baseAI.followPlayerState);
                return;
            }

            if (baseAI.finalDestination == null || baseAI.finalDestination == Vector3.zero) baseAI.finalDestination = baseAI.FindRoamingSpot();


            if (Vector3.Distance(baseAI.transform.position, baseAI.finalDestination) < 0.5f)
            {
                baseAI.finalDestination = baseAI.FindRoamingSpot();
            }

            if (baseAI.agent.destination != baseAI.finalDestination)
                baseAI.agent.SetDestination(baseAI.finalDestination);

            baseAI.HandleRotation();
        }

        public void Exit(BaseAI baseAI) { }
    }
}