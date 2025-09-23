using UnityEngine;

namespace TW
{
    public class AimCurrentPlayer : MonoBehaviour
    {
        EnemyController enemyController;

        [SerializeField]
        Transform debugTarget;

        // Start is called before the first frame update
        void OnEnable()
        {
            if(debugTarget != null)
            {
                transform.LookAt(debugTarget.position);
                return;
            }
            enemyController ??= GetComponentInParent<EnemyController>();

            if (enemyController.BaseAI.currentPlayerInsight != null)
            {
                Vector3 direction = transform.root.position - enemyController.BaseAI.currentPlayerInsight.transform.position;

                Vector3 target = transform.root.transform.position + transform.root.transform.forward * 20;

                float angle = Vector3.SignedAngle(-transform.root.forward, direction, transform.root.up);
                if (angle < 45 && angle > -45)
                {
                    target = enemyController.BaseAI.currentPlayerInsight.transform.position;
                }

                transform.LookAt(target);
            }
        }

        private void LateUpdate() => gameObject.SetActive(false);
    }
}