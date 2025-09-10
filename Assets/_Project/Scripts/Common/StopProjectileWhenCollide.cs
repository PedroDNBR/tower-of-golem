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
            Debug.Log("COLIDIU");
            Debug.Log(projectile);
            if (projectile == null) return;
            Invoke(nameof(StopProjectile), 0.01f);
            
        }

        private void StopProjectile()
        {
            projectile.Speed = 0;
            projectile.Gravity = 0;
            Destroy(projectile);
        }
    }
}
