using UnityEngine;

namespace TW
{
    [CreateAssetMenu(menuName = "GameAssets/Spell")]
    public class Spell : ScriptableObject
    {
        public SpellType type;
        public float damage;
        public float range;
        public float speed;
        public GameObject prefab;
    }
}
