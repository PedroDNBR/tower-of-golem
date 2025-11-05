using UnityEngine;
using System;

namespace TW
{
    public class PoolObject : MonoBehaviour
    {
        public Action onReturnToPool;

        public void ReturnToPool()
        {
            onReturnToPool?.Invoke();
        }
    }
}