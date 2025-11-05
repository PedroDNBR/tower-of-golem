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

        public override void OnEnable()
        {
            base.OnEnable();
            for (int i = 0; i < triggerColliders.Count; i++)
            {
                triggerColliders[i].enabled = true;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (HandleDamage(other))
            {
                if (destroyThisTriggerDetector)
                {
                    Invoke(nameof(DestroyThisTriggerDetector), .1f);
                }
                if (destroyWhenDamage) poolObject.ReturnToPool();
            }
        }

        private void DestroyThisTriggerDetector()
        {
            for (int i = 0; i < triggerColliders.Count; i++)
            {
                triggerColliders[i].enabled = false;
            }
            this.enabled = false;
        }
    }
}