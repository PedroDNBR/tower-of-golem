using TW;
using UnityEngine;

public class DealDamageWhenTriggerEnter : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private Elements element;

    private void OnTriggerEnter(Collider other)
    {
        BaseHealth health = other.GetComponent<BaseHealth>();
        if(health == null)
            health = other.GetComponentInChildren<BaseHealth>();

        if (health == null)
            health = other.GetComponentInParent<BaseHealth>();

        if (health == null) return;

        health.TakeDamage(element, damage);
    }
}
