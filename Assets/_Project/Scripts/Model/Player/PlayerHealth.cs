using UnityEngine;
using UnityEngine.SceneManagement;

namespace TW
{
    public class PlayerHealth : BaseHealth
    {
        public override void TakeDamage(Elements damageType, float damage, GameObject origin)
        {
            base.TakeDamage(damageType, damage, origin);
            if(health <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
