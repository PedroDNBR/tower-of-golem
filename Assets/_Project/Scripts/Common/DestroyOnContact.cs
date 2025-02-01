using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) => Destroy(this.gameObject);
}
