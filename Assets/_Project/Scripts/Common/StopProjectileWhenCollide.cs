using UnityEngine;

namespace TW
{
    public class StopProjectileWhenCollide : MonoBehaviour
    {
        Projectile projectile;

        void OnEnable()
        {
            projectile = GetComponent<Projectile>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (projectile == null) return;
            Invoke(nameof(StopProjectile), 0.01f);

            transform.parent = other.transform;
            
        }

        private void StopProjectile()
        {
            projectile.Speed = 0;
            projectile.Gravity = 0;
            Destroy(projectile);
        }
    }
}
