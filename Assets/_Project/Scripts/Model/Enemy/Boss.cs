namespace TW
{
    public class Boss : BaseAI
    {
        public override void Init()
        {
            base.Init();
            gameObject.SetActive(false);
        }
    }
}
