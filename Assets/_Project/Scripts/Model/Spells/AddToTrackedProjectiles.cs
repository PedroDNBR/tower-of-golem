using UnityEngine;

namespace TW
{
    public class AddToTrackedProjectiles : MonoBehaviour
    {
        public PoolObject poolObject;
        public void AddToTracker(ProjectileTracker tracker, string id)
        {
            tracker.trackedProjectile = transform;
            tracker.id = id;
            tracker.poolObject = poolObject;
        }
    }
}
