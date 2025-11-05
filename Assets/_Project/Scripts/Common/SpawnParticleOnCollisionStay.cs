using UnityEngine;

namespace TW
{
    public class SpawnParticleOnCollisionStay : MonoBehaviour
    {
        public LayerMask layer;

        public string particleName;

        [SerializeField] float spawnInterval = .2f;
        private float nextSpawnTime = 0f;

        private void OnCollisionStay(Collision other)
        {
            if (((1 << other.gameObject.layer) & layer.value) != 0)
            {
                if (Time.time >= nextSpawnTime)
                {
                    Vector3 hitPoint = other.contacts[0].point;
                    ObjectPoolController.instance.InstantiateParticle(particleName, hitPoint, Quaternion.identity);
                    nextSpawnTime = Time.time + spawnInterval;
                }
            }
        }
    }
}
