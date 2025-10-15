namespace TW
{
    public static class CitizenStates
    {
        public static readonly RoamingState roamingState = new RoamingState();
        public static readonly CircleAroundPlayerState circleAroundPlayerState = new CircleAroundPlayerState();
        public static readonly RunAwayFromPlayerState runAwayFromPlayerState = new RunAwayFromPlayerState();
    }
}

