using UnityEngine;

namespace TW
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerDealDamageOnCollision : MonoBehaviour
    {
        Rigidbody rigid;

        [SerializeField]
        protected float minVelocityToDealDamage = 8f;

        [SerializeField]
        protected float damageMultiplier = 1.5f;

        private Elements type;

        public Elements Type { set => type = value; } 

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();  
        }

        private void OnTriggerEnter(Collider other)
        {
            ShouldReceiveDamage shouldReceiveDamage = other.GetComponent<ShouldReceiveDamage>();
            if (shouldReceiveDamage == null) return;
             EnemyHealth enemy = other.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemy == null) return;
            if (rigid == null) return;
            Rigidbody enemyRigid = enemy.GetComponent<Rigidbody>();
            float totalVelocity = rigid.velocity.magnitude - enemyRigid.velocity.magnitude;
            if (totalVelocity < minVelocityToDealDamage) return;

            enemy.TakeDamage(type, totalVelocity * damageMultiplier, gameObject);
        }
    }
}

