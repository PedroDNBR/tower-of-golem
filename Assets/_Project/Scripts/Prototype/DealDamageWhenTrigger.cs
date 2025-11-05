using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
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

        public PoolObject poolObject;

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

        protected virtual bool HandleDamage(Collider other)
        {
            if (NetworkManager.Singleton != null && !NetworkManager.Singleton.IsServer) return false;

            if (!ignoreOrigin)
                if (characterBaseHealth == null && playerController == null) return false;

            ShouldReceiveDamage shouldReceiveDamage = other.GetComponent<ShouldReceiveDamage>();
            //if (shouldReceiveDamage == null) shouldReceiveDamage = other.GetComponentInChildren<ShouldReceiveDamage>();
            //if (shouldReceiveDamage == null) shouldReceiveDamage = other.GetComponentInParent<ShouldReceiveDamage>();

            if (shouldReceiveDamage == null) return false;

            BaseHealth health = other.GetComponent<BaseHealth>();
            if (health == null)
                health = other.GetComponentInChildren<BaseHealth>();

            if (health == null)
                health = other.GetComponentInParent<BaseHealth>();

            if (health == null) return false;

            if (!canDamageOrigin)
            {
                if (characterBaseHealth == health) return false;
            }

            if (playerController != null && health is EnemyHealth)
            {
                (health as EnemyHealth).TakeDamage(element, damage, playerController);
            }
            else
            {
                health.TakeDamage(element, damage, gameObject);
            }

            return true;
        }

        public virtual void OnDisable()
        {
            if (spawnDamageTriggersWhenDestroyed.Count <= 0) return;

            for (int i = 0; i < spawnDamageTriggersWhenDestroyed.Count; ++i)
            {
                var trigger = Instantiate(spawnDamageTriggersWhenDestroyed[i].gameObject, transform.position, transform.rotation);
                DealDamageWhenTrigger dealDamageWhenTrigger = trigger.GetComponent<DealDamageWhenTrigger>();
                if (dealDamageWhenTrigger != null)
                {
                    if (CharacterBaseHealth != null)
                        dealDamageWhenTrigger.CharacterBaseHealth = CharacterBaseHealth;
                    if (playerController != null)
                        dealDamageWhenTrigger.playerController = playerController;
                }
            }
        }
    }
}
