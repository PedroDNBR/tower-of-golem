namespace TW
{
    public class EnemyRoamingState : RoamingState
    {
        public override void Execute(BaseAI baseAI)
        {
            baseAI.TrackPlayer();
            base.Execute(baseAI);
        }
    }
}