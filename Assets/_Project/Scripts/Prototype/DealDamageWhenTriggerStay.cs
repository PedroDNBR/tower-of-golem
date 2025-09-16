using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class DealDamageWhenTriggerStay : DealDamageWhenTrigger
    {

        [SerializeField] float damageInterval = .2f;
        private float nextDamageTime = 0f;

        private void OnTriggerStay(Collider other)
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;

            if (Time.time >= nextDamageTime)
            {
                HandleDamage(other);

                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}