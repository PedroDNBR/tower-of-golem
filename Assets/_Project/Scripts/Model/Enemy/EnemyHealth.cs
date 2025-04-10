using UnityEngine;

namespace TW
{
    public class EnemyHealth : BaseHealth
    {
        [SerializeField]
        protected string hittedAnimNameString = "Hitted";

        [SerializeField]
        private float maxDamageUntilStagger = 20f;

        private float damageUntilStagger = 0;

        private EnemyController enemyController;

        public EnemyController EnemyController { set => enemyController = value; }

        public override void TakeDamage(Elements damageType, float damage, GameObject origin)
        {
            base.TakeDamage(damageType, damage, origin);
            damageUntilStagger += damage * DamageMultiplier.table[type][damageType];

            if (health.Value > 0 && damageUntilStagger > maxDamageUntilStagger)
            {
                enemyController.AnimatorController.PlayTargetAnimation(hittedAnimNameString, true);
                damageUntilStagger = 0;
            }
        }
    }
}
