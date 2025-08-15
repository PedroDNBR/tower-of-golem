using Unity.Netcode;
using UnityEngine;

namespace TW
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerDealDamageOnCollision : NetworkBehaviour
    {
        //private const float damageRadius = .3579978f;

        Rigidbody rigid;

        [SerializeField]
        protected float damageMultiplier = 1.5f;

        private Elements type;

        public Elements Type { set => type = value; }

        // private float totalVelocity = 0;
        public NetworkVariable<float> totalVelocity = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private Vector3 previousPosition;

        private Vector3 detectionPositionOffset = new Vector3(-0.00136566162f, 0.0338081717f, 0.0204851031f);

        bool alreadyDamagedEnemy = false;

        public PlayerController playerController;

        private void Start()
        {
            rigid = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (!IsLocalPlayer) return;
            totalVelocity.Value = ((transform.position - previousPosition) / Time.deltaTime).magnitude;
            previousPosition = transform.position;

            //RaycastHit hit;
            //int layer = LayerMask.GetMask("Hitbox");
            //if (Physics.SphereCast(
            //    transform.position + detectionPositionOffset + ((transform.position - previousPosition).normalized * Mathf.Clamp(totalVelocity * .01f, 0, .175f)),
            //    Constants.playerDealDamageRadius * Mathf.Clamp(totalVelocity * .05f, 1, 1.4f),
            //    Vector3.forward,
            //    out hit,
            //    Constants.playerDealDamageRadius * Mathf.Clamp(totalVelocity * .05f, 1, 1.4f),
            //    layer)
            //)
            //{
            //    ShouldReceiveDamage shouldReceiveDamage = hit.collider.GetComponent<ShouldReceiveDamage>();
            //    if (shouldReceiveDamage == null) return;

            //    EnemyHealth enemy = hit.transform.gameObject.GetComponentInParent<EnemyHealth>();
            //    if (enemy == null) return;

            //    Debug.Log($"Velocity: {this.totalVelocity}");

            //    float totalVelocity = this.totalVelocity - enemy.totalVelocity;
            //    if (totalVelocity < enemy.minVelocityToDealDamage) return;
            //    Debug.Log($"DAMAGE CONFIRMED {totalVelocity}", enemy);

            //    enemy.TakeDamage(type, totalVelocity * damageMultiplier, gameObject);

            //    Vector3 moveDir;
            //    moveDir = rigid.transform.position - enemy.transform.position;
            //    rigid.AddForce(moveDir.normalized * (totalVelocity * .1f), ForceMode.Impulse);

            //}
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsLocalPlayer) return;
            ShouldReceiveDamage shouldReceiveDamage = other.GetComponent<ShouldReceiveDamage>();
            if (shouldReceiveDamage == null) return;

            EnemyHealth enemy = other.transform.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemy == null) return;


            Debug.Log($"Velocity: {this.totalVelocity.Value}");

            float totalVelocity = this.totalVelocity.Value - enemy.totalVelocity;

            NetworkObjectReference networkRef = new NetworkObjectReference(enemy.GetComponent<NetworkObject>());

            ReportCollisionVelocityServerRpc(totalVelocity, networkRef);

            Vector3 moveDir;
            moveDir = rigid.transform.position - enemy.transform.position;
            rigid.AddForce(moveDir.normalized * (totalVelocity * .1f), ForceMode.Impulse);
        }

        [ServerRpc]
        void ReportCollisionVelocityServerRpc(float impactVelocity, NetworkObjectReference enemyHit)
        {
            if (!IsServer) return;
            NetworkObject retrievedObject;
            if (enemyHit.TryGet(out retrievedObject))
            {
                EnemyHealth enemy = retrievedObject.GetComponentInParent<EnemyHealth>();
                if (enemy == null) return;
                if (impactVelocity < enemy.minVelocityToDealDamage) return;
                Debug.Log($"DAMAGE CONFIRMED {impactVelocity}", enemy);

                if (!NetworkManager.Singleton.IsServer) return;
                enemy.TakeDamage(type, impactVelocity * damageMultiplier, playerController);
            }
            
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawWireSphere(
        //        transform.position + detectionPositionOffset + ((transform.position - previousPosition).normalized * Mathf.Clamp(totalVelocity * .01f, 0, .175f)),
        //        Constants.playerDealDamageRadius * Mathf.Clamp(totalVelocity * .05f, 1, 1.4f));
        //    Debug.DrawRay(transform.position, (transform.position - previousPosition).normalized * 5.0f);
        //}
    }
}

