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

        [HideInInspector]
        public PlayerController playerController;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            // this.enabled = IsServer;
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
            try
            {
                if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;

                ShouldReceiveDamage shouldReceiveDamage = other.GetComponent<ShouldReceiveDamage>();
                if (shouldReceiveDamage == null) return;

                BaseHealth health = other.GetComponent<BaseHealth>();
                if (health == null)
                    health = other.GetComponentInChildren<BaseHealth>();

                if (health == null)
                    health = other.GetComponentInParent<BaseHealth>();

                if (health == null) return;

                if (characterBaseHealth == health) return;

                if (playerController != null && health is EnemyHealth)
                {
                    Debug.Log("is playerController and is hitting enemy");
                    (health as EnemyHealth).TakeDamage(element, damage, playerController);
                }
                else
                    health.TakeDamage(element, damage, gameObject);
            }
            finally
            {
                if (destroyWhenDamage) Destroy(gameObject);
            }
        }
    }
}