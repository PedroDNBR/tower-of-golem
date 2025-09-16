using UnityEngine;

namespace TW
{
    public class DealDamageWhenTriggerEnter : DealDamageWhenTrigger
    {
        private void OnTriggerEnter(Collider other)
        {
            HandleDamage(other);
            if (destroyWhenDamage) Destroy(gameObject);
        }
    }
}