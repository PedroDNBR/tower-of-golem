using UnityEngine;

namespace TW
{
    public class SpawnParticleOnCollisionExit : MonoBehaviour
    {
        public LayerMask layer;

        public GameObject spawnParticle;

        private ContactPoint contactPoint;

        private void OnCollisionStay(Collision other)
        {
            if (((1 << other.gameObject.layer) & layer.value) != 0)
            {
                contactPoint = other.contacts[0];
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (((1 << other.gameObject.layer) & layer.value) != 0)
            {
                Vector3 hitPoint = contactPoint.point;
                Instantiate(spawnParticle, hitPoint, Quaternion.identity);
            }
        }
    }
}
