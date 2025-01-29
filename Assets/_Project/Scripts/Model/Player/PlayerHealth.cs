using UnityEngine.SceneManagement;

namespace TW
{
    public class PlayerHealth : BaseHealth
    {
        public override void TakeDamage(Elements damageType, float damage)
        {
            base.TakeDamage(damageType, damage);
            if(health <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }
}
