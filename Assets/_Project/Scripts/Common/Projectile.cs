using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class Projectile : MonoBehaviour
    {
        public float Speed { get => speed; set => speed = value; }
        public float Gravity { get => gravity; set => gravity = value; }

        [SerializeField]
        float speed = 20f;

        [SerializeField]
        float gravity = 5f;

        private void FixedUpdate()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            transform.Rotate(Vector3.right * gravity * Time.deltaTime);
        }
    }
}
