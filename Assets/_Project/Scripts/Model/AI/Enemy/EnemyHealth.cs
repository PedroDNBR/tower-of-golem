using UnityEngine;
using Unity.Netcode;

namespace TW
{
    public class EnemyHealth : BaseHealth
    {
        [SerializeField]
        protected string staggeredAnimNameString = "Staggered";
        [SerializeField]
        protected string damagedAnimNameString = "Damaged";

        [SerializeField]
        private float maxDamageUntilStagger = 20f;

        private float damageUntilStagger = 0;

        private EnemyController enemyController;

        public EnemyController EnemyController { set => enemyController = value; }

        public float minVelocityToDealDamage = 8f;

        public float totalVelocity;

        protected Vector3 previousPosition;

        public bool canBeStaggerd = true;

        public System.Action<float, GameObject>  onDamage;

        protected override void Start()
        {
            InitOnHealthChangedAction();
            if (IsServer)
            {
                maxHealth.Value *= (1 + ((NetworkManager.Singleton.ConnectedClientsList.Count - 1) * .1f));
                health.Value = maxHealth.Value;
            }
        }

        protected virtual void FixedUpdate()
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;
            totalVelocity = ((transform.position - previousPosition) / Time.deltaTime).magnitude;
            previousPosition = transform.position;
        }

        public override void TakeDamage(Elements damageType, float damage, GameObject origin)
        {
            base.TakeDamage(damageType, damage, origin);

            float totalDamage = damage * DamageMultiplier.table[type][damageType];
            if (totalDamage <= 0) return;

            damageUntilStagger += totalDamage;

            if (health.Value > 0 && damageUntilStagger > maxDamageUntilStagger && canBeStaggerd)
            {
                enemyController.AnimatorController.PlayTargetAnimation(staggeredAnimNameString, true);
                damageUntilStagger = 0;
            }
            else
            {
                enemyController.AnimatorController.PlayTargetAnimation(damagedAnimNameString);
            }

            onDamage?.Invoke(damage, origin);
        }

        public void TakeDamage(Elements damageType, float damage, PlayerController player)
        {
            TakeDamage(damageType, damage, player.gameObject);
            enemyController.BaseAI.RegisterDamage(player, damage);
        }
    }
}
