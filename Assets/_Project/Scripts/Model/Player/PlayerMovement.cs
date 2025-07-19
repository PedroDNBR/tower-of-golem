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

        private NetworkVariable<float> dashTimerCount = new NetworkVariable<float>(0f);

        private NetworkVariable<bool> isGrounded = new NetworkVariable<bool>(false);

        // current / max
        public event Action<float, float> StaminaChanged;

        public void Init()
        {
            rigid = GetComponentInChildren<Rigidbody>();
            if (IsLocalPlayer && IsOwner)
            {
                dashTimerCount.OnValueChanged += (float previous, float current) => {
                    StaminaChanged?.Invoke(dashTimerCount.Value, dashTime.Value);
                };
            }
            SetStaminaServerRpc();
            
        }

        [ServerRpc]
        public void SetStaminaServerRpc()
        {
            dashTimerCount.Value = dashTime.Value;
        }

        private void Update()
        {
            if (!IsServer) return;
            CalculateIsGrounded();
            CalculateDashTimer();
        }

        public void Movement(float horizontal, float vertical, float delta, Transform reference)
        {
            if (horizontal == 0 && vertical == 0) return;
            if (!isGrounded.Value) return;
            Vector3 camRight = reference.right;
            Vector3 camForward = reference.forward;
            camForward.y = 0; // Evita movimentos verticais
            camForward.Normalize();

            // Cria o vetor de direção combinando input com orientação da câmera
            Vector3 moveDir = (camRight * vertical + camForward * -horizontal).normalized;

            // Aplica torque com base nessa direção
            rigid.AddTorque(moveDir * speed.Value * delta * 10);
        }

        public void Jump()
        {
            if (dashTimerCount.Value < dashCost.Value) return;
            if (!isGrounded.Value) return;

            rigid.AddForce(Vector3.up * jump.Value, ForceMode.Impulse);
            ConsumeStaminaServerRpc();
        }

        [ServerRpc]
        private void ConsumeStaminaServerRpc()
        {
            dashTimerCount.Value -= dashCost.Value;
        }

        public void Dash(float horizontal, float vertical, Transform reference)
        {
            if (dashTimerCount.Value < dashCost.Value) return;
            Vector3 camRight = reference.right;
            Vector3 camForward = reference.forward;
            camForward.y = 0; // Evita movimentos verticais
            camForward.Normalize();

            // Cria o vetor de direção combinando input com orientação da câmera
            Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;

            // Aplica torque com base nessa direção
            rigid.AddForce(moveDir * dash.Value, ForceMode.Impulse);
            ConsumeStaminaServerRpc();
        }

        private void CalculateIsGrounded()
        {
            RaycastHit hit;
            Vector3 origin = transform.position;
            origin.y += groundDetectionRayStartPoint;

            if (Physics.Raycast(origin, -Vector3.up, out hit, minimumDistanceNeededToBeginFall, groundLayer))
                isGrounded.Value = true;
            else
                isGrounded.Value = false;
        }

        private void CalculateDashTimer()
        {
            if (dashTimerCount.Value < dashTime.Value)
            {
                dashTimerCount.Value += Time.deltaTime * dashRegen.Value;
            }
        }

        public Rigidbody GetRigidbody() => rigid;
    }
}

