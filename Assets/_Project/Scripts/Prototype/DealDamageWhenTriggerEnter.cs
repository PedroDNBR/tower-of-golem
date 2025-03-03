using TW;
using UnityEngine;

public class DealDamageWhenTriggerEnter : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private Elements element;

    BaseHealth characterBaseHealth;

    public BaseHealth CharacterBaseHealth { set => characterBaseHealth = value; get => characterBaseHealth; }

    [SerializeField]
    bool destroyWhenDamage = false;

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
