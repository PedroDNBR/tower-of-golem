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

            if (!baseAI.isBusy)
            {
                if (!baseAI.actionFlag)
                {
                    Vector3 dir = baseAI.currentPlayerInsight.transform.position - baseAI.transform.position;
                    dir.y = 0;
                    dir.Normalize();

                    float angle = Vector2.Angle(baseAI.transform.position, dir);
                    baseAI.currentSnapshot = baseAI.GetAction(dist, angle);
                    Debug.Log("FollowPlayerState baseAI.currentSnapshot = baseAI.GetAction(dist, angle)");
                    Debug.Log(baseAI.currentSnapshot);

                    if (baseAI.currentSnapshot != null)
                    {
                        Debug.Log("FollowPlayerState SwitchState(States.attackPlayerState)");
                        baseAI.SwitchState(States.attackPlayerState);
                        return;
                    }

                }
            }
            Debug.Log("FollowPlayerState SetDestination and HandleRotation");
            baseAI.agent.SetDestination(baseAI.currentPlayerInsight.transform.position);
            baseAI.HandleRotation();
        }

        public void Exit(BaseAI baseAI) { }
    }
}