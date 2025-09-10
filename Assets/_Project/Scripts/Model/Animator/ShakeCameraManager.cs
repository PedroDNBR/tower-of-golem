using System;
using System.Collections.Generic;
using UnityEngine;

namespace TW
{
    public class ShakeCameraManager : MonoBehaviour
    {
        [SerializeField]
        public List<CameraShakeInfo> cameraShakeInfos = new List<CameraShakeInfo>();

        public void ShakeCamera(int i)
        {
            CameraShake.Instance.ShakeCamera(cameraShakeInfos[i].strength, cameraShakeInfos[i].shakeTime, cameraShakeInfos[i].shakeVelocity);
        }
    }

    [Serializable]
    public class CameraShakeInfo
    {
        public float strength;
        public float shakeTime;
        public float shakeVelocity;
    }
}
