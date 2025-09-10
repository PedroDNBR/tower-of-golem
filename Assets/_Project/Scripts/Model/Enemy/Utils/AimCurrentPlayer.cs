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
                transform.LookAt(enemyController.BaseAI.currentPlayerInsight.transform.position);
        }

        private void LateUpdate() => gameObject.SetActive(false);
    }
}