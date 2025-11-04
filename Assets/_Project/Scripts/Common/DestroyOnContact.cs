using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DestroyOnContact : MonoBehaviour
{
    public LayerMask mask;

    public Action onDestroyObject;

    public Coroutine destroyCoroutine;

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
        onDestroyObject?.Invoke();
    }
}
