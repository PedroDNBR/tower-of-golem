using UnityEngine;

namespace TW
{
    public class ChangeHealthValuesAnimator : MonoBehaviour
    {
        public EnemyHealth enemyHealth;

        public void CanBeStaggered() => enemyHealth.canBeStaggerd = true;

        public void CannotBeStaggered() => enemyHealth.canBeStaggerd = false;
    }
}
