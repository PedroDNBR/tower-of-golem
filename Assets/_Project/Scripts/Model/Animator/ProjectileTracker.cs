using UnityEngine;

namespace TW
{
    public class ProjectileTracker : MonoBehaviour
    {
        public Transform trackedProjectile;

        public void DestroyTrackedProjectile()
        {
            if(trackedProjectile != null) Destroy(trackedProjectile.gameObject);
        }
    }
}