using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class ChangeHealthValuesAnimator : MonoBehaviour
    {
        public EnemyHealth enemyHealth;

        public void CanBeStaggered()
        {
            if (NetworkManager.Singleton.IsServer)
                enemyHealth.canBeStaggerd = true;
        }

        public void CannotBeStaggered()
        {
            if (NetworkManager.Singleton.IsServer)
                enemyHealth.canBeStaggerd = false;
        }
    }
}
