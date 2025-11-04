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

        protected override void Start()
        {
            base.Start();
            for (int i = 0; i < triggerColliders.Count; i++)
            {
                triggerColliders[i].enabled = true;
            }
            this.enabled = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (HandleDamage(other))
            {
                if (destroyThisTriggerDetector)
                {
                    Invoke(nameof(DestroyThisTriggerDetector), .1f);
                }
                if (destroyWhenDamage) onDestroyObject?.Invoke();
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