using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    protected Animator animator;

    protected const string isBusyString = "isBusy";
    protected const string movementString = "movement";
    
    [HideInInspector]
    public bool canRotate = true;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMovementValue(float value) => animator.SetFloat(movementString, value);

    public bool GetIsBusyBool() => animator.GetBool(isBusyString);

    public void PlayTargetAnimation(string animationName, bool isBusy)
    {
        animator.SetBool(isBusyString, isBusy);
        animator.CrossFade(animationName, 0.1f);
    }

    public void CanRotate(int canRotate) => this.canRotate = canRotate == 1 ? true : false;

    public bool GetCanRotate() => canRotate;
}
