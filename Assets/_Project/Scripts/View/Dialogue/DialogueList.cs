using System;
using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    [CreateAssetMenu(menuName = "TW/GameAssets/Dialogue List")]
    public class DialogueList : ScriptableObject
    {
        public List<DialogueData> dialogues = new List<DialogueData>();
    }

    [Serializable]
    public class DialogueData
    {
        public Character character;
        public string dialogueIndex;
    }
}

