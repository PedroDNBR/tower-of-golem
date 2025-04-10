using UnityEngine;

namespace TW
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float cameraHeight = 8.9f;
        [SerializeField] private float cameraDistance = -18.9f;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Vector3 cameraRotation;

        [SerializeField] private Camera playerCamera;
        public Camera PlayerCameraObj { get => playerCamera; set => playerCamera = value; }

        private void Start()
        {
            if (cameraTransform == null)
                return;

            cameraTransform.eulerAngles = cameraRotation;
        }

        private void LateUpdate()
        {
            if (cameraTransform == null)
                return;

            cameraTransform.position = new Vector3(transform.position.x, cameraHeight, transform.position.z + cameraDistance);
        }
    }
}
