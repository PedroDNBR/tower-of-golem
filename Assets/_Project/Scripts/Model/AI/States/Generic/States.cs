namespace TW
{
    public static class States
    {
        public static readonly AttackPlayerState attackPlayerState = new AttackPlayerState();
        public static readonly RoamingState roamingState = new RoamingState();
        public static readonly RoamingState enemyRoamingState = new EnemyRoamingState();
        public static readonly FollowPlayerState followPlayerState = new FollowPlayerState();
        public static readonly CircleAroundPlayerState circleAroundPlayerState = new CircleAroundPlayerState();
        public static readonly UnarmedState unarmedState = new UnarmedState();

        public static readonly DogChasePlayerState dogChasePlayerState = new DogChasePlayerState();
    }
}
