using UnityEngine;

namespace TW
{
    public class SpawnParticleOnCollisionEnter : MonoBehaviour
    {
        public LayerMask layer;

        public string particleName;

        private void OnCollisionEnter(Collision other)
        {
            if (((1 << other.gameObject.layer) & layer.value) != 0)
            {
                Vector3 hitPoint = other.contacts[0].point;
                ObjectPoolController.instance.InstantiateParticle(particleName, hitPoint, Quaternion.identity);
            }
        }
    }
}
