using UnityEngine;
using Unity.Netcode;

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

        public float minVelocityToDealDamage = 8f;

        public float totalVelocity;

        protected Vector3 previousPosition;

        protected virtual void FixedUpdate()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            totalVelocity = ((transform.position - previousPosition) / Time.deltaTime).magnitude;
            previousPosition = transform.position;
        }

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
