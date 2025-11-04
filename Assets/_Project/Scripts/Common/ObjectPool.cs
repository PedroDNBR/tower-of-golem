using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TW
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool instance;

        private void Awake() => instance = this;

        [SerializeField]
        List<SpellTypeGameObject> availableSpells = new List<SpellTypeGameObject>();
        
        Dictionary<string, GameObject> availableSpellsDictonary = new Dictionary<string, GameObject>();

        Dictionary<string, List<GameObject>> spellsPool = new Dictionary<string, List<GameObject>>();

        private void Start()
        {
            foreach (var spell in availableSpells)
            {
                availableSpellsDictonary.Add(spell.spellId, spell.prefab);
            }
        }

        public GameObject InstantiateSpell(string spellId, Vector3 targetPosition, Quaternion targetRotation)
        {
            GameObject instantiatedSpell = null;
            if(spellsPool.ContainsKey(spellId))
            {
                if (spellsPool[spellId].Count > 0)
                {
                    spellsPool[spellId][0].transform.position = targetPosition;
                    spellsPool[spellId][0].transform.rotation = targetRotation;
                    spellsPool[spellId][0].SetActive(true);
                    instantiatedSpell = spellsPool[spellId][0];
                    spellsPool[spellId].Remove(spellsPool[spellId][0]);
                    return instantiatedSpell;
                }
            }
            instantiatedSpell = Instantiate(availableSpellsDictonary[spellId], targetPosition, targetRotation);
            


            DealDamageWhenTrigger dealDamageWhenTrigger = instantiatedSpell.GetComponent<DealDamageWhenTrigger>();
            if(dealDamageWhenTrigger != null) dealDamageWhenTrigger.onDestroyObject += () => AddSpellToPool(spellId, instantiatedSpell);

            DestroyOnTime destroyOnTime = instantiatedSpell.GetComponent<DestroyOnTime>();
            if (destroyOnTime != null) destroyOnTime.onDestroyObject += () => AddSpellToPool(spellId, instantiatedSpell);

            DestroyOnContact destroyOnContact = instantiatedSpell.GetComponent<DestroyOnContact>();
            if (destroyOnContact != null) destroyOnContact.onDestroyObject += () => AddSpellToPool(spellId, instantiatedSpell);

            return instantiatedSpell;
        }

        public void AddSpellToPool(string spellId, GameObject spellPrefab)
        {
            if (spellsPool.ContainsKey(spellId))
                spellsPool[spellId].Add(spellPrefab);
            else
                spellsPool.Add(spellId, new List<GameObject> { spellPrefab });

            spellPrefab.SetActive(false);
        }
    }

    [Serializable]
    public class SpellTypeGameObject
    {
        public string spellId;
        public GameObject prefab;
    }
}
