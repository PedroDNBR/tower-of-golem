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
            EnemyHealth enemy = other.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemy == null) return;
            if (rigid == null) return;
            if (rigid.velocity.magnitude < minVelocityToDealDamage) return;

            enemy.TakeDamage(type, rigid.velocity.magnitude * damageMultiplier, gameObject);
        }
    }
}

