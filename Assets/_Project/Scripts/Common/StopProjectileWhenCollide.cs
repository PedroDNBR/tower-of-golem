using UnityEngine;

namespace TW
{
    public class StopProjectileWhenCollide : MonoBehaviour
    {
        Projectile projectile;
        public Transform origin;
        public LayerMask ignoreLayer;

        void OnEnable()
        {
            projectile = GetComponent<Projectile>();
            DealDamageWhenTriggerEnter damageWhenTriggerEnter = GetComponent<DealDamageWhenTriggerEnter>();
            if (damageWhenTriggerEnter != null)
                if (damageWhenTriggerEnter.CharacterBaseHealth != null)
                    origin = damageWhenTriggerEnter.CharacterBaseHealth.transform.root;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(origin == null)
            {
                DealDamageWhenTriggerEnter damageWhenTriggerEnter = GetComponent<DealDamageWhenTriggerEnter>();
                if (damageWhenTriggerEnter != null)
                    origin = damageWhenTriggerEnter.CharacterBaseHealth.transform.root;
            }

            if (other.transform.root == origin) return;
            if (((1 << other.gameObject.layer) & ignoreLayer.value) == 0) return;
            if (projectile == null) return;
            Invoke(nameof(StopProjectile), 0.01f);

            transform.parent = other.transform;
            Rigidbody rigid = GetComponent<Rigidbody>();
            if (rigid != null)
                Destroy(rigid);
        }

        private void StopProjectile()
        {
            projectile.Speed = 0;
            projectile.Gravity = 0;
            Destroy(projectile);
        }
    }
}
