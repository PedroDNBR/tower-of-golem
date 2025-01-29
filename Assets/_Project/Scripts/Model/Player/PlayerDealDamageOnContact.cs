using TW;
using UnityEngine;

public class PlayerDealDamageOnContact : MonoBehaviour
{
    [SerializeField]
    private float damage = 10f;

    public float Damage { get => damage; set => damage = value; }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colidiu");
        BaseAI enemy = other.gameObject.GetComponentInParent<BaseAI>();
        if (enemy == null) return;

        enemy.TakeDamage(damage);
    }
}
