using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
namespace TW
{
    public class DealDamageWhenTrigger : MonoBehaviour
    {
        [SerializeField] protected bool canDamageOrigin = true;
        [SerializeField] protected float damage;
        [SerializeField] protected Elements element;

        protected BaseHealth characterBaseHealth;

        public BaseHealth CharacterBaseHealth { set => characterBaseHealth = value; get => characterBaseHealth; }

        [SerializeField]
        protected bool destroyWhenDamage = false;

        [SerializeField]
        protected bool ignoreOrigin = false;

        [HideInInspector]
        public PlayerController playerController;

        [SerializeField]
        protected List<DealDamageWhenTrigger> spawnDamageTriggersWhenDestroyed;

        public virtual void OnEnable()
        {
            this.enabled = NetworkManager.Singleton == null ? true : NetworkManager.Singleton.IsServer;
        }

        protected virtual void Start()
        {
            if (characterBaseHealth == null)
                characterBaseHealth = GetComponentInChildren<BaseHealth>();

            if (characterBaseHealth == null)
                characterBaseHealth = GetComponentInParent<BaseHealth>();

            if (characterBaseHealth == null)
                characterBaseHealth = GetComponent<BaseHealth>();
        }

        protected virtual void HandleDamage(Collider other)
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return;

            if(!ignoreOrigin)
                if (characterBaseHealth == null && playerController == null) return;

            ShouldReceiveDamage shouldReceiveDamage = other.GetComponent<ShouldReceiveDamage>();
            if (shouldReceiveDamage == null) return;

            BaseHealth health = other.GetComponent<BaseHealth>();
            if (health == null)
                health = other.GetComponentInChildren<BaseHealth>();

            if (health == null)
                health = other.GetComponentInParent<BaseHealth>();

            if (health == null) return;

            if (!canDamageOrigin)
                if (characterBaseHealth == health) return;

            if (playerController != null && health is EnemyHealth)
            {
                (health as EnemyHealth).TakeDamage(element, damage, playerController);
            }
            else
                health.TakeDamage(element, damage, gameObject);
        }

        protected virtual void OnDestroy()
        {
            if (spawnDamageTriggersWhenDestroyed.Count <= 0) return;

            for (int i = 0; i < spawnDamageTriggersWhenDestroyed.Count; ++i)
            {
                var trigger = Instantiate(spawnDamageTriggersWhenDestroyed[i].gameObject, transform.position, transform.rotation);
                DealDamageWhenTriggerEnter dealDamageWhenTriggerEnter = trigger.GetComponent<DealDamageWhenTriggerEnter>();
                if (dealDamageWhenTriggerEnter != null)
                {
                    if (CharacterBaseHealth != null)
                        dealDamageWhenTriggerEnter.CharacterBaseHealth = CharacterBaseHealth;
                    if (playerController != null)
                        dealDamageWhenTriggerEnter.playerController = playerController;
                }

                DealDamageWhenTriggerStay dealDamageWhenTriggerStay = trigger.GetComponent<DealDamageWhenTriggerStay>();
                if (dealDamageWhenTriggerStay != null)
                {
                    if (CharacterBaseHealth != null)
                        dealDamageWhenTriggerStay.CharacterBaseHealth = CharacterBaseHealth;
                    if (playerController != null)
                        dealDamageWhenTriggerStay.playerController = playerController;
                }
                
            }
        }
    }
}
