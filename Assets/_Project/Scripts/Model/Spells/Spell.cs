using UnityEngine;

namespace TW
{
    [CreateAssetMenu(menuName = "GameAssets/Spell")]
    public class Spell : ScriptableObject
    {
        public Elements type;
        public float damage;
        public float range;
        public float speed;
        public float gravity;
        public GameObject prefab;
    }
}
