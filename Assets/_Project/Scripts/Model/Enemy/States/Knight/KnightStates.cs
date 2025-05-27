namespace TW
{
    public static class KnightStates
    {
        public static readonly KnightFollowPlayerState followPlayerState = new KnightFollowPlayerState();
        public static readonly KnightAttackPlayerState attackPlayerState = new KnightAttackPlayerState();
        public static readonly KnightAvoidingHitState avoidingHitState = new KnightAvoidingHitState();
        public static readonly KnightBackingOffState backingOffState = new KnightBackingOffState();
    }
}
