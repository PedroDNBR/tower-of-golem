using System;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField] NetworkVariable<float> speed = new NetworkVariable<float>(5f);
        [SerializeField] NetworkVariable<float> jump = new NetworkVariable<float>(5f);
        [SerializeField] NetworkVariable<float> dash = new NetworkVariable<float>(5f);
        [SerializeField] NetworkVariable<float> dashTime = new NetworkVariable<float>(1.3f);
        [SerializeField] NetworkVariable<float> dashCost = new NetworkVariable<float>(0.5f);
        [SerializeField] NetworkVariable<float> dashRegen = new NetworkVariable<float>(0.5f);
        [SerializeField] private Transform groundChecker;
        [SerializeField] private float groundDetectionRayStartPoint;
        [SerializeField] private float minimumDistanceNeededToBeginFall;

        [SerializeField] LayerMask groundLayer;

        public Vector3 movementInput;

        private Rigidbody rigid;

        private float dashTimerCount = 0;

        private bool isGrounded = false;

        // current / max
        public event Action<float, float> StaminaChanged;

        public void Init()
        {
            rigid = GetComponentInChildren<Rigidbody>();
        }

        private void Update()
        {
            if (!IsLocalPlayer) return;
            CalculateIsGrounded();
            CalculateDashTimer();
        }

        public void Movement(float horizontal, float vertical, float delta)
        {
            if (horizontal == 0 && vertical == 0) return;
            if (!isGrounded) return;
            Vector3 velocity = new Vector3(vertical, 0, -horizontal);
            rigid.AddTorque(velocity * speed.Value * delta * 10);
        }

        public void Jump()
        {
            if (dashTimerCount < dashCost.Value) return;
            if (!isGrounded) return;

            rigid.AddForce(Vector3.up * jump.Value, ForceMode.Impulse);
            ConsumeStamina();
        }

        private void ConsumeStamina()
        {
            dashTimerCount -= dashCost.Value;
            InvokeStaminaChanged();
        }

        public void Dash(float horizontal, float vertical)
        {
            if (dashTimerCount < dashCost.Value) return;
            Vector3 dashVelocity = new Vector3(horizontal, 0, vertical);
            rigid.AddForce(dashVelocity * dash.Value, ForceMode.Impulse);
            ConsumeStamina();
        }

        private void CalculateIsGrounded()
        {
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += groundDetectionRayStartPoint;

            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, groundLayer))
                isGrounded = true;
            else
                isGrounded = false;

            Debug.DrawRay(origin, -Vector3.up * minimumDistanceNeededToBeginFall, Color.red);
        }

        private void CalculateDashTimer()
        {
            if (dashTimerCount < dashTime.Value)
            {
                dashTimerCount += Time.deltaTime * dashRegen.Value;
                InvokeStaminaChanged();
            }
        }

        protected void InvokeStaminaChanged() => StaminaChanged?.Invoke(dashTimerCount, dashTime.Value);

        public Rigidbody GetRigidbody() => rigid;
    }
}

