using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    [CreateAssetMenu(menuName = "TW/GameAssets/Characters")]
    public class Character : ScriptableObject
    {
        public string characterName;
        public Elements type;
        public Sprite image;
        public List<string> dialogues;
    }
}
