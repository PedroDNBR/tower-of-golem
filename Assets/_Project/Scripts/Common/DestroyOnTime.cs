using System;
using System.Collections;
using UnityEngine;

namespace TW
{
    public class DestroyOnTime : MonoBehaviour
    {
        [SerializeField]
        float timer = 2f;

        [SerializeField]
        MonoBehaviour objectToDestroy;

        [SerializeField]
        bool usePool;

        public Action onDestroyObject;

        private Coroutine DestroyCoroutine;

        private void OnEnable()
        {
            if(!usePool)
            {
                Destroy(objectToDestroy == null ? gameObject : objectToDestroy, timer);
                return;
            }
            DestroyCoroutine = StartCoroutine(AddToPoolOnTime());
        }

        IEnumerator AddToPoolOnTime()
        {
            yield return new WaitForSeconds(timer);
            onDestroyObject?.Invoke();
        }

        private void OnDisable()
        {
            if (DestroyCoroutine != null)
                DestroyCoroutine = null;
        }
    }
}
