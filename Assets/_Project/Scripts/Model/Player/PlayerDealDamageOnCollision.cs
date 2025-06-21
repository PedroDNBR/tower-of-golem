using Unity.Netcode;
using UnityEngine;

namespace TW
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerDealDamageOnCollision : MonoBehaviour
    {
        //private const float damageRadius = .3579978f;
        private const float damageRadius = .4f;

        Rigidbody rigid;

        [SerializeField]
        protected float damageMultiplier = 1.5f;

        private Elements type;

        public Elements Type { set => type = value; }

        private float totalVelocity = 0;

        private Vector3 previousPosition;

        private Vector3 detectionPositionOffset = new Vector3(-0.00136566162f, 0.0338081717f, 0.0204851031f);

        bool alreadyDamagedEnemy = false;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();  
        }

        private void FixedUpdate()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            totalVelocity = ((transform.position - previousPosition) / Time.deltaTime).magnitude;
            previousPosition = transform.position;

            if (!NetworkManager.Singleton.IsServer) return;

            if (totalVelocity < 2f) return;

            RaycastHit hit;
            int layer = LayerMask.GetMask("Hitbox");
            if (Physics.SphereCast(transform.position + detectionPositionOffset, damageRadius, Vector3.forward, out hit, damageRadius, layer))
            {
                if (alreadyDamagedEnemy) return;
                ShouldReceiveDamage shouldReceiveDamage = hit.collider.GetComponent<ShouldReceiveDamage>();
                if (shouldReceiveDamage == null) return;

                EnemyHealth enemy = hit.transform.gameObject.GetComponentInParent<EnemyHealth>();
                if (enemy == null) return;

                float totalVelocity = this.totalVelocity - enemy.totalVelocity;
                if (totalVelocity < enemy.minVelocityToDealDamage) return;

                enemy.TakeDamage(type, totalVelocity * damageMultiplier, gameObject);

                //Vector3 moveDir;
                //moveDir = rigid.transform.position - enemy.transform.position;
                //rigid.AddForce(moveDir.normalized * 5.5f, ForceMode.Impulse);

                alreadyDamagedEnemy = true;
            } 
            else
            {
                if(alreadyDamagedEnemy) alreadyDamagedEnemy = false;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position + detectionPositionOffset, damageRadius);
            Debug.DrawRay(transform.position, (transform.position - previousPosition).normalized * 5.0f);
        }
    }
}

