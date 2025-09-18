using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


namespace TW
{
    public class EnableDisableObjectsAnimator : MonoBehaviour
    {
        [SerializeField]
        private List<GameObjectName> gameObjects = new List<GameObjectName>();

        Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
        Dictionary<string, bool> objectsShouldNotDisable = new Dictionary<string, bool>();

        private void Start()
        {
            //if (!NetworkManager.Singleton.IsServer) return;
            for (int i = 0; i < gameObjects.Count; i++)
            {
                objects.Add(gameObjects[i].colliderName, gameObjects[i].gObject);
                objectsShouldNotDisable.Add(gameObjects[i].colliderName, gameObjects[i].shouldNotDisable);
            }
        }

        public void EnableGameObject(string name) => SetGameObjects(name, true);

        public void DisableGameObject(string name) => SetGameObjects(name, false);

        private void SetGameObjects(string name, bool isEnabled)
        {
            if (!NetworkManager.Singleton.IsServer) return;
            string[] objectNames = name.Split(",");
            for (int i = 0; i < objectNames.Length; i++)
                objects[objectNames[i]].SetActive(isEnabled);
        }

        public void DestroyAllColliders()
        {
            for (int i = 0; i < gameObjects.Count; i++)
                Destroy(gameObjects[i].gObject);
        }

        public void DisableAll()
        {
            Debug.Log("DISABLE EnableDisableObjectsAnimator");
            foreach (var item in objects)
            {
                if(!objectsShouldNotDisable[item.Key])
                    DisableGameObject(item.Key);
            }
        }

        [Serializable]
        class GameObjectName
        {
            public string colliderName;
            public GameObject gObject;
            public bool shouldNotDisable = false;
        }
    }
}
