using System;
using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class SpawnOnDestroy : MonoBehaviour
    {
        [SerializeField]
        List<ObjectsToSpawn> objects = new List<ObjectsToSpawn>();

        private void OnDestroy()
        {
            foreach (var obj in objects)
                Instantiate(obj.objectToSpawn, 
                    transform.position + obj.offsetPosition, 
                    transform.rotation * obj.offsetRotation
                );
        }
    }

    [Serializable]
    public class ObjectsToSpawn
    {
        public GameObject objectToSpawn;
        public Vector3 offsetPosition;
        public Quaternion offsetRotation = new Quaternion(1,1,1,1);
    }
}
