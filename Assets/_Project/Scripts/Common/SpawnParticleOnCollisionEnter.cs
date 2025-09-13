using UnityEngine;

namespace TW
{
    public class SpawnParticleOnCollisionEnter : MonoBehaviour
    {
        public LayerMask layer;

        public GameObject spawnParticle;

        private void OnCollisionEnter(Collision other)
        {
            if (((1 << other.gameObject.layer) & layer.value) != 0)
            {
                Vector3 hitPoint = other.contacts[0].point;
                Instantiate(spawnParticle, hitPoint, Quaternion.identity);
            }
        }
    }
}
