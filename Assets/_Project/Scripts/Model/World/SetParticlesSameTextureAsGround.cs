using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class SetParticlesSameTextureAsGround : MonoBehaviour
    {
        [SerializeField]
        List<ParticleSystemRenderer> particleSystems = new List<ParticleSystemRenderer>();

        public LayerMask targetLayers;

        private void OnEnable()
        {
            RaycastHit hit;

            if (Physics.Raycast(transform.position + (transform.up * 2), -transform.up, out hit, 10f, targetLayers))
            {
                // Get the surface normal at the hit point
                Vector3 surfaceNormal = hit.normal;

                // Calculate the dot product of the ray direction and the surface normal
                float dotProduct = Vector3.Dot(-transform.up, surfaceNormal);


                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal);
                transform.position = hit.point;

                MeshRenderer meshRenderer = hit.collider.GetComponent<MeshRenderer>();
                if(meshRenderer != null)
                {
                    for (int i = 0; i < particleSystems.Count; ++i)
                    {
                        particleSystems[i].material = meshRenderer.material;
                    }
                }
            }
        }
    }
}
