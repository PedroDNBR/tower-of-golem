using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class DealDamageWhenTriggerEnter : DealDamageWhenTrigger
    {
        [SerializeField]
        protected bool destroyThisTriggerDetector = false;

        [SerializeField]
        List<Collider> triggerColliders = new List<Collider>(); 
        private void OnTriggerEnter(Collider other)
        {
            HandleDamage(other);
            if (destroyThisTriggerDetector)
            {
                Invoke(nameof(DestroyThisTriggerDetector), .1f);
            }
            if (destroyWhenDamage) Destroy(gameObject);

        }

        private void DestroyThisTriggerDetector()
        {
            Destroy(this);
            for (int i = 0; i < triggerColliders.Count; i++)
            {
                Destroy(triggerColliders[i]);
            }
        }
    }
}