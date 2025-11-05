using System.Collections;
using TW;
using UnityEngine;

public class DestroyOnContact : MonoBehaviour
{
    public LayerMask mask;

    public Coroutine destroyCoroutine;

    public PoolObject poolObject;

    private void OnEnable()
    {
        if (destroyCoroutine != null)
            destroyCoroutine = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((mask.value & (1 << other.transform.gameObject.layer)) > 0)
        {
            destroyCoroutine = StartCoroutine(DestroyObject());
        }
    }

    private IEnumerator DestroyObject()
    {
        yield return new WaitForEndOfFrame();
        destroyCoroutine = null;
        poolObject.ReturnToPool();
    }
}
