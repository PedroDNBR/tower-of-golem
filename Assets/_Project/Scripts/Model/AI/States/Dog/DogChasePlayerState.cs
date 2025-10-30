namespace TW
{
    public class DogChasePlayerState : FollowPlayerState
    {
        float cachedSpeed;

        public override void Enter(BaseAI baseAI)
        {
            base.Enter(baseAI);
            cachedSpeed = baseAI.walkSpeed;
            baseAI.walkSpeed *= 3.7f;
            if (AICommander.Instance != null)
                AICommander.Instance.Register(baseAI as Melee);
        }

        public override void Exit(BaseAI baseAI)
        {
            base.Exit(baseAI);
            baseAI.walkSpeed = cachedSpeed;
        }
    }
}

