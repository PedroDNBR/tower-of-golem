using UnityEngine;

namespace TW
{
    public class AimCurrentPlayer : MonoBehaviour
    {
        EnemyController enemyController;
        // Start is called before the first frame update
        void OnEnable()
        {
            enemyController ??= GetComponentInParent<EnemyController>();

            if (enemyController.BaseAI.currentPlayerInsight != null)
                transform.LookAt(enemyController.BaseAI.currentPlayerInsight.transform.position);
        }

        private void LateUpdate() => gameObject.SetActive(false);
    }
}