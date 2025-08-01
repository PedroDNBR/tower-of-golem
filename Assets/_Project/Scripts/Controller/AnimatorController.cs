using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace TW
{
    public class AnimatorController : NetworkBehaviour
    {
        protected Animator animator;

        protected NavMeshAgent agent;

        [HideInInspector]
        public bool canRotate = true;
        [HideInInspector]
        public bool isBusy;

        public NavMeshAgent Agent { set => agent = value; }

        public Animator Animator { get => animator; }


        public void Init()
        {
            animator = GetComponent<Animator>();
            animator.applyRootMotion = true;
        }

        private void Update()
        {
            if (!IsServer || animator == null) return;
            isBusy = GetIsBusyBool();
            animator.applyRootMotion = isBusy;
        }

        public void SetMovementValue(float horizontal, float vertical)
        {
            if (!IsServer) return;
            animator.SetFloat("horizontal", horizontal);
            animator.SetFloat("vertical", vertical);
        }

        public bool GetIsBusyBool() => animator.GetBool(Constants.isBusyString);

        public void PlayTargetAnimation(string animationName, bool isBusy)
        {
            animator.SetBool(Constants.isBusyString, isBusy);
            animator.CrossFade(animationName, 0);
        }

        public void CanRotate(int canRotate) => this.canRotate = canRotate == 1 ? true : false;

        public bool GetCanRotate() => canRotate;

        public void OnAnimatorMove()
        {
            if(isBusy)
                agent.nextPosition += animator.deltaPosition;
        }
    }
}
