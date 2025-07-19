using Unity.Netcode;
using UnityEngine;

namespace TW
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerDealDamageOnCollision : MonoBehaviour
    {
        //private const float damageRadius = .3579978f;

        Rigidbody rigid;

        [SerializeField]
        protected float damageMultiplier = 1.5f;

        [SerializeField]
        Transform daamgeColliderPosition;

        private Elements type;

        public Elements Type { set => type = value; }

        private float playerTotalVelocity = 0;

        private Vector3 previousPosition;

        bool alreadyDamagedEnemy = false;

        int layer;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
            layer = LayerMask.GetMask("Hitbox");
        }

        private void FixedUpdate()
        {
            if (!NetworkManager.Singleton.IsServer) return;
            playerTotalVelocity = ((transform.position - previousPosition) / Time.deltaTime).magnitude;
            previousPosition = transform.position;

            if (!NetworkManager.Singleton.IsServer) return;
            if (playerTotalVelocity < 2f) return;

            RaycastHit hit;
            if (Physics.SphereCast(daamgeColliderPosition.position, Constants.playerDealDamageRadius, Vector3.forward, out hit, Constants.playerDealDamageRadius, layer))
            {
                if (alreadyDamagedEnemy) return;
                ShouldReceiveDamage shouldReceiveDamage = hit.collider.GetComponent<ShouldReceiveDamage>();
                if (shouldReceiveDamage == null) return;

                EnemyHealth enemy = hit.transform.gameObject.GetComponentInParent<EnemyHealth>();
                if (enemy == null) return;

                Debug.Log($"playerTotalVelocity {playerTotalVelocity}");
                Debug.Log($"enemy.totalVelocity {enemy.totalVelocity}");

                float totalVelocity = playerTotalVelocity - enemy.totalVelocity;
                Debug.Log($"totalVelocity {totalVelocity}");
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
            Gizmos.DrawWireSphere(daamgeColliderPosition.position, Constants.playerDealDamageRadius);
            Debug.DrawRay(transform.position, (transform.position - previousPosition).normalized * 5.0f);
        }
    }
}

