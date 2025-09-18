using UnityEngine;

namespace TW
{
    public class UnarmedState : IAIState
    {
        private const string RECOVER_WEAPON_ANIM_NAME = "RecoverWeapon";

        public void Enter(BaseAI baseAI)
        {
            if (
                !baseAI.enemyController.AnimatorController.GetIsBusyBool() && 
                !baseAI.actionFlag &&
                (baseAI as Boss).AreAllWeaponsAreDisabled()
                )
            {
                baseAI.enemyController.AnimatorController.PlayTargetAnimation(RECOVER_WEAPON_ANIM_NAME);
            }
        }

        public void Execute(BaseAI baseAI)
        {
            if (
                !baseAI.enemyController.AnimatorController.GetIsBusyBool() &&
                !baseAI.actionFlag &&
                (baseAI as Boss).AreAllWeaponsAreDisabled()
                )
            {
                baseAI.enemyController.AnimatorController.PlayTargetAnimation(RECOVER_WEAPON_ANIM_NAME);
            }

            if(!(baseAI as Boss).AreAllWeaponsAreDisabled()) baseAI.SwitchState(States.followPlayerState);
        }

        public void Exit(BaseAI baseAI)
        {
            
        }
    }
}