
namespace TW
{
    public class Archer : BaseAI
    {
        public override void Init()
        {
            base.Init();
            // enemyController.EnemyHealth.Dead += () => AICommander.Instance.Unregister(this);
            playerFound += (PlayerController playerController) =>
            {
                if (enemyController.BaseAI.currentPlayerInsight != playerController)
                {
                    enemyController.AnimatorController.PlayTargetAnimation("Draw", true);
                }
            };
        }

        protected override void SetFollowPlayerState() => followPlayerState = States.followPlayerState;
    }

}
