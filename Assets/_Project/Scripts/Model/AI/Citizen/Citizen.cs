using UnityEngine;

namespace TW
{
    public class Citizen : BaseAI
    {
        public override void Init()
        {
            base.Init();
            enemyController.EnemyHealth.onDamage += SwitchToRunAwayFromPlayerState;
        }

        protected void SwitchToRunAwayFromPlayerState(float damage, GameObject origin)
        {
            PlayerController playerController = origin.GetComponent<PlayerController>();
            if (playerController == null) playerController = origin.GetComponentInChildren<PlayerController>();
            if (playerController == null) playerController = origin.GetComponentInParent<PlayerController>();
            if (playerController == null) return;
            currentPlayerInsight = playerController;
            SwitchState(CitizenStates.runAwayFromPlayerState);
        }

        public override void UpdateAnimation()
        {
            Vector3 velocity = agent.velocity;

            velocity.y = 0;

            Vector3 moveDir = velocity.normalized;

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            float forwardAmount = Vector3.Dot(moveDir, forward);
            float strafeAmount = Vector3.Dot(moveDir, right);

            float speed = velocity.magnitude;
            forwardAmount *= Mathf.Clamp01(speed / walkSpeed);
            strafeAmount *= Mathf.Clamp01(speed / walkSpeed);

            if(currentState == CitizenStates.runAwayFromPlayerState)
                enemyController.AnimatorController.SetMovementValue(strafeAmount, forwardAmount * 2);
            else
                enemyController.AnimatorController.SetMovementValue(strafeAmount, forwardAmount);

            Vector3 target = currentPlayerInsight != null ? currentPlayerInsight.transform.position : agent.destination;
            Vector3 direction = transform.position - target;
            float angle = Vector3.SignedAngle(-transform.forward, direction, transform.up); // Angle around Y-axis
            float normalizedAngle = angle / 180f; // Scales angle from -180 to 180 to -1 to 1

            enemyController.AnimatorController.SetRotationValue(normalizedAngle);
        }
    }
}