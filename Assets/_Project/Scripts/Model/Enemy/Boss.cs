using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class Boss : BaseAI
    {
        [SerializeField]
        public List<Transform> weaponsTransform;

        public override void Init()
        {
            base.Init();
        }

        private void Start()
        {
            StartCoroutine(DrawPlayerToAttack());
        }

        public override void SwitchState(IAIState newState)
        {
            base.SwitchState(newState);

            if (AreAllWeaponsAreDisabled() && currentState != States.unarmedState)
                SwitchState(States.unarmedState);
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

        public bool AreAllWeaponsAreDisabled()
        {
            foreach (var item in weaponsTransform)
            {
                if (item.gameObject.activeSelf)
                    return false;
            }

            return true;
        }
    }
}
