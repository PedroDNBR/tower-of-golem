using System.Threading.Tasks;
using UnityEngine;

namespace TW
{
    public class CameraShake : MonoBehaviour
    {
        public static CameraShake Instance;

        public CameraShake() => Instance = this;

        public Camera cameraToShake;

        private Vector3 currentShakeOffset;

        public async void ShakeCamera(float strength, float shakeTime, float shakeVelocity)
        {
            await Shake(strength, shakeTime, shakeVelocity);
        }

        private async Task Shake(float strength, float shakeTime, float shakeVelocity)
        {
            float timer = shakeTime;
            float interval = 1f / shakeVelocity; // tempo entre novos alvos
            float intervalTimer = 0f;

            Vector3 targetOffset = Vector3.zero;

            while (timer > 0)
            {
                float currentIntensity = Mathf.Lerp(0, strength, timer / shakeTime);

                intervalTimer -= Time.deltaTime;
                if (intervalTimer <= 0f)
                {
                    targetOffset = Random.insideUnitSphere * currentIntensity;
                    intervalTimer = interval;
                }

                currentShakeOffset = Vector3.Lerp(
                    currentShakeOffset,
                    targetOffset,
                    Time.deltaTime * shakeVelocity * 2f
                );

                transform.localPosition = currentShakeOffset;

                timer -= Time.deltaTime;
                await Task.Yield();
            }

            // Smooth return to zero
            float smoothReturnTime = 0.2f;
            Vector3 currentCamPos = transform.localPosition;
            float elapsed = 0f;
            while (elapsed < smoothReturnTime)
            {
                transform.localPosition = Vector3.Lerp(currentCamPos, Vector3.zero, elapsed / smoothReturnTime);
                elapsed += Time.deltaTime;
                await Task.Yield();
            }
            transform.localPosition = Vector3.zero;
            currentShakeOffset = Vector3.zero;
        }
    }
}
