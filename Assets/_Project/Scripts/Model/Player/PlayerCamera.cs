using UnityEngine;

namespace TW
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float cameraHeight = 8.9f;
        [SerializeField] private float cameraDistance = -18.9f;
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private Transform cameraHolder;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Vector3 cameraRotation;

        [SerializeField] private Camera playerCamera;
        public Camera PlayerCameraObj { get => playerCamera; set => playerCamera = value; }

        [SerializeField] float edgeThresholdX = 0.2f;
        [SerializeField] float edgeThresholdY = 0.2f;
        [SerializeField] float maxSpeed = 500f;
        [SerializeField] float acceleration = 500f;

        public Vector3 OverrideCameraPosition;

        [SerializeField] Vector2 cameraXPositionClamp = Vector2.zero;
        [SerializeField] Vector2 cameraZPositionClamp = Vector2.zero;

        private void Start()
        {
            if (cameraTransform == null && cameraPivot == null)
                return;

            cameraHolder.eulerAngles = cameraRotation;
            Vector3 newCameraPosition = new Vector3(transform.position.x, transform.position.y + cameraHeight, transform.position.z + cameraDistance);
            cameraPivot.position = newCameraPosition;
        }

        //private void LateUpdate()
        //{
        //    if (cameraTransform == null)
        //        return;

        //    Vector3 newCameraPosition;
        //    if (OverrideCameraPosition == Vector3.zero)
        //        newCameraPosition = new Vector3(transform.position.x, transform.position.y + cameraHeight, transform.position.z + cameraDistance);
        //    else
        //        newCameraPosition = OverrideCameraPosition;

        //    cameraTransform.position = Vector3.Lerp(cameraTransform.position, newCameraPosition, Time.deltaTime * 10);
        //}

        float radiusX = 0.65f; // raio horizontal da elipse (mais baixo = mais sensível lateral)
        float radiusY = 0.65f; // raio vertical da elipse

        public void MoveCamera(Vector2 mousePosition)
        {
            Vector3 newCameraPosition = new Vector3(transform.position.x, transform.position.y + cameraHeight, transform.position.z + cameraDistance);
            cameraPivot.position = Vector3.Lerp(cameraPivot.position, newCameraPosition, Time.deltaTime * 10);
            
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;

            Vector2 center = new Vector2(screenWidth / 2f, screenHeight / 2f);
            Vector2 normalized = (mousePosition - center);
            normalized.x /= screenWidth / 2f;
            normalized.y /= screenHeight / 2f;

            float ellipseEquation = Mathf.Pow(normalized.x / radiusX, 2) + Mathf.Pow(normalized.y / radiusY, 2);
            bool wantsToMove = ellipseEquation > 1f;

            // Checa se o player está fora da zona segura (em world space)
            Vector3 cameraPos = cameraHolder.localPosition;

            Vector3 moveDirection = new Vector3(normalized.x, 0f, normalized.y).normalized;
            float t = Mathf.Clamp01((ellipseEquation - 1f) * 0.5f);
            float speed = Mathf.SmoothStep(0f, maxSpeed, t);
            Vector3 velocity = moveDirection * speed;
            Vector3 newPosition = cameraPos + velocity * Time.deltaTime;
            Vector3 lerpPosition = Vector3.Lerp(cameraPos, newPosition, Time.deltaTime * acceleration);
            lerpPosition.x = Mathf.Clamp(lerpPosition.x, cameraXPositionClamp.x, cameraXPositionClamp.y);
            lerpPosition.z = Mathf.Clamp(lerpPosition.z, cameraZPositionClamp.x, cameraZPositionClamp.y);
            cameraHolder.localPosition = lerpPosition;
        }
    }
}
