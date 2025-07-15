namespace TW
{
    public class BossController : EnemyController
    {
        protected BossArea bossArea;

        public BossArea BossArea { get => bossArea; }

        protected override void OnEnable()
        {
            base.OnEnable();
            bossArea = FindObjectOfType<BossArea>();
            baseAI.enabled = false;
        }

        protected override void Start()
        {
            base.Start();
            if (bossArea != null) bossArea.boss = this;
        }
    }
}
