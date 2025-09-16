using UnityEngine;

namespace TW
{
    public class DestroyOnTime : MonoBehaviour
    {
        [SerializeField]
        float timer = 2f;

        [SerializeField]
        MonoBehaviour objectToDestroy;

        private void Start() => Destroy(objectToDestroy == null ? gameObject : objectToDestroy, timer);
    }
}
