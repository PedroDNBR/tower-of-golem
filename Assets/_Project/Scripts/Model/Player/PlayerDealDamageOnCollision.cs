using UnityEngine;

namespace TW
{
    public class PlayerDealDamageOnCollision : MonoBehaviour
    {
        Rigidbody rigid;

        [SerializeField]
        protected float minVelocityToDealDamage = 8f;

        [SerializeField]
        protected float damageMultiplier = 1.5f;


        private void Start()
        {
            rigid = GetComponent<Rigidbody>();  
        }

        private void OnTriggerEnter(Collider other)
        {
            BaseAI enemy = other.gameObject.GetComponentInParent<BaseAI>();
            if (enemy == null) return;
            if (rigid == null) return;
            if (rigid.velocity.magnitude < .5f) return;

            Debug.Log($"Deal Damage {rigid.velocity.magnitude * damageMultiplier}");


            enemy.TakeDamage(rigid.velocity.magnitude * damageMultiplier);
        }
    }
}

