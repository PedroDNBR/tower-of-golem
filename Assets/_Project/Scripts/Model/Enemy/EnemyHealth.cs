using UnityEngine;

namespace TW
{
    public class EnemyHealth : BaseHealth
    {
        [SerializeField]
        protected string hittedAnimNameString = "Hitted";

        private EnemyController enemyController;

        public EnemyController EnemyController { set => enemyController = value; }

        public override void TakeDamage(Elements damageType, float damage)
        {
            base.TakeDamage(damageType, damage);
            enemyController.AnimatorController.PlayTargetAnimation(hittedAnimNameString, true);
        }
    }
}
