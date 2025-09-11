using UnityEngine;

namespace TW
{
    public class AddToTrackedProjectiles : MonoBehaviour
    {
        public void AddToTracker(ProjectileTracker tracker)
        {
            tracker.trackedProjectile = transform;
        }
    }
}
