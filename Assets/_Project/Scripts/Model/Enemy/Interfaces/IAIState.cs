namespace TW
{
    public interface IAIState
    {
        void Enter(BaseAI baseAI);
        void Execute(BaseAI baseAI);
        void Exit(BaseAI baseAI);
    }
}
