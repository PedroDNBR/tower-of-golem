using System;
using System.Reflection;
using UnityEngine;

namespace TW
{
    public class SwitchStateAnimator : MonoBehaviour
    {
        protected EnemyController enemyController;
        public EnemyController EnemyController
        {
            get => enemyController;
            set => enemyController = value; 
        }

        public void SwitchAIState(string state)
        {
            Type statesType = typeof(States);

            FieldInfo field = statesType.GetField(state, BindingFlags.Public | BindingFlags.Static);

            if (field != null)
            {
                object value = field.GetValue(null);
                enemyController.BaseAI.SwitchState(value as IAIState);
            }
            else
            {
            }
        }
    }
}