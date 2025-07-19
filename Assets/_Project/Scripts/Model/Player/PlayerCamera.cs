using UnityEngine;

namespace TW
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private float cameraHeight = 8.9f;
        [SerializeField] private float cameraDistance = -18.9f;
        [SerializeField] private Transform cameraHorizontalPivot;
        [SerializeField] private Transform cameraVerticalPivot;
        [SerializeField] private Vector3 cameraRotation;

        public Transform cameraReference { get => cameraHorizontalPivot; }

        [SerializeField] private Camera playerCamera;
        public Camera PlayerCameraObj { get => playerCamera; set => playerCamera = value; }

        public Vector3 OverrideCameraPosition;

        private void Start()
        {
            if (cameraHorizontalPivot == null)
                return;

            //cameraTransform.eulerAngles = cameraRotation;
            //Vector3 newCameraPosition = new Vector3(transform.position.x, transform.position.y + cameraHeight, transform.position.z + cameraDistance);
            //cameraTransform.position = newCameraPosition;

            cameraHorizontalPivot.position = transform.position;

        }

        private void LateUpdate()
        {
            if (cameraHorizontalPivot == null)
                return;

            //Vector3 newCameraPosition;
            //if (OverrideCameraPosition == Vector3.zero)
            //    newCameraPosition = new Vector3(transform.position.x, transform.position.y + cameraHeight, transform.position.z + cameraDistance);
            //else
            //    newCameraPosition = OverrideCameraPosition;

            cameraHorizontalPivot.position = Vector3.Lerp(cameraHorizontalPivot.position, transform.position, Time.deltaTime * 10);


        }

        float xRotation;

        internal void MoveCamera(Vector2 mouseAim)
        {
            //var lookDirection = mouseAim;
            //var mouse = lookDirection * Time.deltaTime * 5;

            //cameraTransform.Rotate(Vector3.up * mouse.x);

            //this.xRotation -= mouse.y;

            //cameraTransform.transform.localRotation = Quaternion.Euler(xRotation, cameraTransform.transform.localRotation.y, 0);
            cameraHorizontalPivot.eulerAngles -= new Vector3(0, -mouseAim.x, 0) * Time.deltaTime * 10;
            cameraVerticalPivot.eulerAngles -= new Vector3(mouseAim.y, 0, 0) * Time.deltaTime * 10;
        }
    }
}
