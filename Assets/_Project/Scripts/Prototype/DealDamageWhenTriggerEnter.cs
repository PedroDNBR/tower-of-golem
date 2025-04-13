using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class DealDamageWhenTriggerEnter : NetworkBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private Elements element;

        BaseHealth characterBaseHealth;

        public BaseHealth CharacterBaseHealth { set => characterBaseHealth = value; get => characterBaseHealth; }

        [SerializeField]
        bool destroyWhenDamage = false;

        protected void OnNetworkSpawn()
        {
            this.enabled = IsServer;
        }

        private void Start()
        {
            if (characterBaseHealth == null)
                characterBaseHealth = GetComponentInChildren<BaseHealth>();

            if (characterBaseHealth == null)
                characterBaseHealth = GetComponentInParent<BaseHealth>();

            if (characterBaseHealth == null)
                characterBaseHealth = GetComponent<BaseHealth>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            ShouldReceiveDamage shouldReceiveDamage = other.GetComponent<ShouldReceiveDamage>();
            if (shouldReceiveDamage == null) return;

            BaseHealth health = other.GetComponent<BaseHealth>();
            if(health == null)
                health = other.GetComponentInChildren<BaseHealth>();

            if (health == null)
                health = other.GetComponentInParent<BaseHealth>();

            if (health == null) return;

            if(characterBaseHealth == health) return;

            health.TakeDamage(element, damage, gameObject);

            if (destroyWhenDamage) Destroy(gameObject);
        }
    }
}