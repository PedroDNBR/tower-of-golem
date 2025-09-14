using System.Collections;
using UnityEngine;

namespace TW
{
    public class Boss : BaseAI
    {
        public override void Init()
        {
            base.Init();
        }

        private void Start()
        {
            StartCoroutine(DrawPlayerToAttack());
        }

        private IEnumerator DrawPlayerToAttack()
        {
            while (true)
            {
                yield return new WaitForSeconds(3f);

                var bestTarget = GetBestTarget();
                if (bestTarget != null)
                    currentPlayerInsight = bestTarget;
            }
        }
    }
}
