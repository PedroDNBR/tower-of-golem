using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace TW
{
    public class ObjectPoolController : MonoBehaviour
    {
        public static ObjectPoolController instance;

        private void Awake() => instance = this;

        [SerializeField]
        List<SpellTypeGameObject> availableSpells = new List<SpellTypeGameObject>();
        
        Dictionary<string, GameObject> availableSpellsDictonary = new Dictionary<string, GameObject>();

        public Dictionary<string, IObjectPool<PoolObject>> spellsPool = new Dictionary<string, IObjectPool<PoolObject>>();

        [SerializeField]
        List<SpellTypeGameObject> availableParticles = new List<SpellTypeGameObject>();

        Dictionary<string, GameObject> availableParticlesDictonary = new Dictionary<string, GameObject>();

        public Dictionary<string, IObjectPool<PoolObject>> particlesPool = new Dictionary<string, IObjectPool<PoolObject>>();

        // Good memories
        //private IObjectPool<GameObject> _pool;

        //private string _spellId;

        private void Start()
        {
            SetupSpellPool();
            SetupParticlePool();
        }

        private void SetupSpellPool()
        {
            foreach (var spell in availableSpells)
            {
                availableSpellsDictonary.Add(spell.spellId, spell.prefab);

                spellsPool.Add(
                    spell.spellId,
                    new ObjectPool<PoolObject>(
                            createFunc: () => CreateItem(spell.spellId, availableSpellsDictonary, spellsPool),
                            actionOnGet: OnGet,
                            actionOnRelease: OnRelease,
                            actionOnDestroy: OnDestroyItem,
                            collectionCheck: true,   // helps catch double-release mistakes
                            defaultCapacity: spell.defaultCapacity,
                            maxSize: spell.maxSize
                        )
                    );
            }
        }

        private void SetupParticlePool()
        {
            foreach (var particle in availableParticles)
            {
                availableParticlesDictonary.Add(particle.spellId, particle.prefab);

                particlesPool.Add(
                    particle.spellId,
                    new ObjectPool<PoolObject>(
                            createFunc: () => CreateItem(particle.spellId, availableParticlesDictonary, particlesPool),
                            actionOnGet: OnGet,
                            actionOnRelease: OnRelease,
                            actionOnDestroy: OnDestroyItem,
                            collectionCheck: true,   // helps catch double-release mistakes
                            defaultCapacity: particle.defaultCapacity,
                            maxSize: particle.maxSize
                        )
                    );
            }
        }

        private PoolObject CreateItem(string itemId, Dictionary<string, GameObject> availableItemsDictionary, Dictionary<string, IObjectPool<PoolObject>> itemPool)
        {
            GameObject instantiatedSpell = Instantiate(availableItemsDictionary[itemId], Vector3.zero, Quaternion.identity);
            PoolObject poolObject = instantiatedSpell.GetComponent<PoolObject>();
            poolObject.onReturnToPool += () => itemPool[itemId].Release(poolObject);
            return poolObject;
        }

        private void OnGet(PoolObject poolObject)
        {
            poolObject.gameObject.SetActive(true);
        }

        // Called when an item is returned to the pool.
        private void OnRelease(PoolObject poolObject)
        {
            poolObject.gameObject.SetActive(false);
        }

        // Called when the pool decides to destroy an item (e.g., above max size).
        private void OnDestroyItem(PoolObject poolObject)
        {
            Destroy(poolObject.gameObject);
        }

        public PoolObject InstantiateSpell(string spellId, Vector3 targetPosition, Quaternion targetRotation)
        {
            var pooledObject = spellsPool[spellId].Get();
            pooledObject.transform.position = targetPosition;
            pooledObject.transform.rotation = targetRotation;
            return pooledObject;
        }

        public PoolObject InstantiateParticle(string particleId, Vector3 targetPosition, Quaternion targetRotation)
        {
            var pooledObject = particlesPool[particleId].Get();
            pooledObject.transform.position = targetPosition;
            pooledObject.transform.rotation = targetRotation;
            return pooledObject;
        }
    }

    [Serializable]
    public class PoolGameObject
    {
        public GameObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 50;
    }

    [Serializable]
    public class SpellTypeGameObject : PoolGameObject
    {
        public string spellId;
    }

    [Serializable]
    public class ParticleGameObject : PoolGameObject
    {
        public string particleId;
    }
}
