using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class DealDamageWhenTriggerStay : MonoBehaviour
    {
        [SerializeField] private float damage;
        [SerializeField] private Elements element;

        BaseHealth characterBaseHealth;

        public BaseHealth CharacterBaseHealth { set => characterBaseHealth = value; get => characterBaseHealth; }

        [HideInInspector]
        public PlayerController playerController;

        [SerializeField] float damageInterval = .2f;
        private float nextDamageTime = 0f;

        public void OnEnable()
        {
            this.enabled = NetworkManager.Singleton == null ? true : NetworkManager.Singleton.IsServer;
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

        private void OnTriggerStay(Collider other)
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;

            if (Time.time >= nextDamageTime)
            {
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
                    (health as EnemyHealth).TakeDamage(element, damage, playerController);
                }
                else
                    health.TakeDamage(element, damage, gameObject);

                nextDamageTime = Time.time + damageInterval;
            }
        }
    }
}