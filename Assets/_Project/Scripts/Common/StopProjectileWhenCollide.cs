using Steamworks;
using UnityEngine;

namespace TW
{
    public class StopProjectileWhenCollide : MonoBehaviour
    {
        Projectile projectile;
        private bool stopExecution;
        public Transform origin;
        public LayerMask ignoreLayer;
        [SerializeField]
        public float delayToStop = 0.01f;

        [SerializeField]
        Vector3 centerDetection;

        [SerializeField]
        Vector3 extentDetection;

        private float cachedProjectileSpeed;
        private float cachedProjectileGravity;

        void OnEnable()
        {
            if(projectile == null)
            {
                projectile = GetComponent<Projectile>();
                cachedProjectileSpeed = projectile.Speed;
                cachedProjectileGravity = projectile.Gravity;
            }
            else
            {
                projectile.Speed = cachedProjectileSpeed;
                projectile.Gravity = cachedProjectileGravity;
                projectile.enabled = true;
            }

            DealDamageWhenTriggerEnter damageWhenTriggerEnter = GetComponent<DealDamageWhenTriggerEnter>();

            if (damageWhenTriggerEnter != null)
                if (damageWhenTriggerEnter.CharacterBaseHealth != null)
                    origin = damageWhenTriggerEnter.CharacterBaseHealth.transform.root;

            stopExecution = false;

        }

        private void FixedUpdate()
        {
            if (projectile == null || stopExecution) return;
            if (origin == null)
            {
                DealDamageWhenTriggerEnter damageWhenTriggerEnter = GetComponent<DealDamageWhenTriggerEnter>();
                if (damageWhenTriggerEnter != null)
                    if (damageWhenTriggerEnter.CharacterBaseHealth != null)
                        origin = damageWhenTriggerEnter.CharacterBaseHealth.transform.root;
            }

            Vector3 worldCenter = transform.position + transform.rotation * centerDetection;

            Collider[] overlaps = Physics.OverlapBox(
                worldCenter,
                extentDetection / 2,
                transform.rotation,
                ignoreLayer
            );

            Transform hitboxTransform = null;
            Transform scenarioTransform = null;

            foreach (var other in overlaps)
            {
                if (other.transform.root == origin) continue;

                if (other.gameObject.layer == LayerMask.NameToLayer("Hitbox"))
                    hitboxTransform = other.transform;
                else if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("BossGround"))
                    scenarioTransform = other.transform;
            }

            if (hitboxTransform != null || scenarioTransform != null)
            {
                if (hitboxTransform != null)
                    transform.parent = hitboxTransform;
                else if (scenarioTransform != null)
                    transform.parent = scenarioTransform;

                if (delayToStop >= 0)
                    Invoke(nameof(StopProjectile), delayToStop);
                else
                    StopProjectile();

                stopExecution = true;
            }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Vector3 worldCenter = transform.position + transform.rotation * centerDetection;
            Gizmos.matrix = Matrix4x4.TRS(worldCenter, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, extentDetection); // full size
        }

        private void StopProjectile()
        {
            projectile.Speed = 0;
            projectile.Gravity = 0;
            projectile.enabled = false;
        }
    }
}
