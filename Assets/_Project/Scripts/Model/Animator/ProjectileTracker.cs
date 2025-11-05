using UnityEngine;

namespace TW
{
    public class ProjectileTracker : MonoBehaviour
    {
        public Transform trackedProjectile;
        public string id;
        public PoolObject poolObject;

        public void DestroyTrackedProjectile()
        {
            if (!string.IsNullOrEmpty(id) && trackedProjectile != null)
            {
                Debug.Log(poolObject, this);
                ObjectPoolController.instance.spellsPool[id].Release(poolObject);
                return;
            }
            if (trackedProjectile != null) Destroy(trackedProjectile.gameObject);
        }
    }
}