using TW;
using UnityEngine;

public class DealDamageWhenTriggerEnter : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private Elements element;

    BaseHealth characterBaseHealth;

    private void Start()
    {
        characterBaseHealth = GetComponentInChildren<BaseHealth>();

        if (characterBaseHealth == null)
            characterBaseHealth = GetComponentInParent<BaseHealth>();

        if (characterBaseHealth == null)
            characterBaseHealth = GetComponent<BaseHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        BaseHealth health = other.GetComponent<BaseHealth>();
        if(health == null)
            health = other.GetComponentInChildren<BaseHealth>();

        if (health == null)
            health = other.GetComponentInParent<BaseHealth>();

        if (health == null) return;

        if(characterBaseHealth == health) return;

        Debug.Log($"Apply damage : {damage}");

        health.TakeDamage(element, damage);
    }
}
