using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TW
{
    public class SpawnOnDestroy : MonoBehaviour
    {
        [SerializeField]
        List<GameObjectsToSpawn> objects = new List<GameObjectsToSpawn>();

        [SerializeField]
        List<PoolObjectsToSpawn> poolObjects = new List<PoolObjectsToSpawn>();

        private void OnDestroy()
        {
            Spawn();
        }

        private void OnDisable()
        {
            Spawn();
        }

        private void Spawn()
        {
            foreach (var obj in objects)
                Instantiate(obj.objectToSpawn,
                    transform.position + obj.offsetPosition,
                    transform.rotation * obj.offsetRotation
                );

            foreach (var obj in poolObjects)
            {
                MethodInfo method = ObjectPoolController.instance.GetType().GetMethod(obj.instantiateFunctionName);
                method.Invoke(
                    ObjectPoolController.instance,
                    new object[] {
                            obj.id, 
                            transform.position + obj.offsetPosition,
                            transform.rotation * obj.offsetRotation 
                        }
                    );
            }
        }
    }

    [Serializable]
    public abstract class ObjectsToSpawn
    {
        public Vector3 offsetPosition;
        public Quaternion offsetRotation = new Quaternion(1,1,1,1);
    }

    [Serializable]
    public class GameObjectsToSpawn : ObjectsToSpawn
    {
        public GameObject objectToSpawn;
    }

    [Serializable]
    public class PoolObjectsToSpawn : ObjectsToSpawn
    {
        public string instantiateFunctionName;
        public string id;
    }
}
