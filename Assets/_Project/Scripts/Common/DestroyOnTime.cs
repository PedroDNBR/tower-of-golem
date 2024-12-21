using UnityEngine;

namespace TW
{
    public class DestroyOnTime : MonoBehaviour
    {
        [SerializeField]
        float timer = 2f;

        private void Start() => Destroy(gameObject, timer);
    }
}
