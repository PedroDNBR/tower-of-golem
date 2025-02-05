using UnityEngine;
using System.Collections;

namespace TW
{
    public class EnemyHealth : BaseHealth
    {
        [SerializeField]
        protected string hittedAnimNameString = "Hitted";

        [SerializeField]
        private float timeUntilStaggerAgain = 2f;

        private float staggerRecoveryTime;

        private bool canStagger = true;

        private EnemyController enemyController;

        public EnemyController EnemyController { set => enemyController = value; }

        public override void TakeDamage(Elements damageType, float damage, GameObject origin)
        {
            base.TakeDamage(damageType, damage, origin);

            Debug.Log(canStagger);

            if (health > 0 && canStagger)
            {
                enemyController.AnimatorController.PlayTargetAnimation(hittedAnimNameString, true);
                canStagger = false;

                 StartCoroutine(CalculateStagger());
            }
        }

        private IEnumerator CalculateStagger()
        {
            yield return new WaitForSeconds(timeUntilStaggerAgain);
            canStagger = true;
        }
    }
}
