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

        public PoolObject poolObject;

        private Coroutine DestroyCoroutine;

        private void OnEnable()
        {
            if(poolObject == null)
            {
                Destroy(objectToDestroy == null ? gameObject : objectToDestroy, timer);
                return;
            }
            DestroyCoroutine = StartCoroutine(AddToPoolOnTime());
        }

        IEnumerator AddToPoolOnTime()
        {
            yield return new WaitForSeconds(timer);
            poolObject.ReturnToPool();
        }

        private void OnDisable()
        {
            if (DestroyCoroutine != null)
                DestroyCoroutine = null;
        }
    }
}
