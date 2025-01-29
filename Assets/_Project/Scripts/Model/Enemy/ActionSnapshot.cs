using UnityEngine;

namespace TW
{
    [System.Serializable]
    [CreateAssetMenu(menuName = "AI/Enemy attacks")]
    public class ActionSnapshot : ScriptableObject
    {
        public string anim;
        public int score = 5;
        public float recoveryTime;
        public float minDist = 2;
        public float maxDist = 5;
        public float minAngle = -35;
        public float maxAngle = 35;
    }
}
