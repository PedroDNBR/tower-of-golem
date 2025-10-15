using Unity.Netcode;

namespace TW
{
    public class BossHealth : EnemyHealth
    {
        protected override void Start()
        {
            InitOnHealthChangedAction();
            if (IsServer)
            {
                maxHealth.Value *= (1 + ((NetworkManager.Singleton.ConnectedClientsList.Count - 1) * .8f));
                health.Value = maxHealth.Value;
            }
        }        
    }
}
