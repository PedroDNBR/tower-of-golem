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
            Debug.Log("Colliding with");
            Debug.Log(other);
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;
            Debug.Log("It is server");

            if (Time.time >= nextDamageTime)
            {
                Debug.Log("Time do Deal Damage");
                HandleDamage(other);

                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}