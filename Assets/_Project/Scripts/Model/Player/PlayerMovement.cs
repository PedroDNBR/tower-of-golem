using System;
using UnityEngine;

namespace TW
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float jump = 2f;
        [SerializeField] private float dash = 5f;
        [SerializeField] float dashTime = 1.3f;
        [SerializeField] private Transform groundChecker;
        [SerializeField] private float groundDetectionRayStartPoint;
        [SerializeField] private float minimumDistanceNeededToBeginFall;

        [SerializeField] LayerMask groundLayer;

        private Rigidbody rigid;

        private float dashTimerCount = 0;

        private bool isGrounded = false;

        // current / max
        public event Action<float, float> StaminaChanged;

        public void Init()
        {
            rigid = GetComponent<Rigidbody>();
            dashTimerCount = dashTime;
        }

        public void Movement(float horizontal, float vertical, float delta)
        {
            if (!isGrounded) return;
            Vector3 velocity = new Vector3(vertical, 0, -horizontal);
            rigid.AddTorque(velocity * speed * delta * 10);
        }

        public void Jump()
        {
            if(isGrounded)
                rigid.AddForce(Vector3.up * jump, ForceMode.Impulse);
        }

        public void Dash(float horizontal, float vertical)
        {
            if (dashTimerCount < dashTime) return;
            Vector3 dashVelocity = new Vector3(horizontal, 0, vertical);
            rigid.AddForce(dashVelocity * dash, ForceMode.Impulse);
            dashTimerCount = 0;
        }

        private void Update()
        {
            CalculateIsGrounded();
            CalculateDashTimer();
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
        }

        private void CalculateDashTimer()
        {
            if (dashTimerCount < dashTime)
            {
                dashTimerCount += Time.deltaTime;
                StaminaChanged.Invoke(dashTimerCount, dashTime);
            }
        }
    }
}

