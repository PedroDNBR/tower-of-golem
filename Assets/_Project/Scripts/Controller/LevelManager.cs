using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class LevelManager : NetworkBehaviour
    {
        public static LevelManager instance;

        public LevelManager() { instance = this; }

        public GameObject bossArenaOusideWalls;
        public GameObject bossArenaInsideWalls;

        public GameObject bossArenaLeftDoor;
        public GameObject bossArenaRightDoor;

        [Header("Left Door")]
        public float leftDoorClosedRotation = 180;
        public float leftDoorOpenRotation = -60;

        [Header("Right Door")]
        public float doorClosedRotation = 180;
        public float doorOpenRotation = -300;

        private void Start()
        {
            if(IsServer)
                BossArea.instance.BossSpawned += () => CloseDoorsOnBossFightStartServerRpc(.5f);

        }

        public async Task OpenDoorsLerp(float lerpTime)
        {
            Vector3 leftRotation = new Vector3(-90, 0, leftDoorOpenRotation);
            Vector3 rightRotation = new Vector3(-90, 0, doorOpenRotation);

            await LerpDoors(leftRotation, rightRotation, lerpTime);
        }

        public async Task CloseDoorsLerp(float lerpTime)
        {
            BossArea.instance.transform.localScale = BossArea.instance.startScale;
            Vector3 leftRotation = new Vector3(-90, 0, leftDoorClosedRotation);
            Vector3 rightRotation = new Vector3(-90, 0, doorClosedRotation);

            await LerpDoors(leftRotation, rightRotation, lerpTime);
        }

        private async Task LerpDoors(Vector3 leftRotation, Vector3 rightRotation, float lerpTime)
        {
            float currentLerpTime = 0f;

            Quaternion leftStart = bossArenaLeftDoor.transform.rotation;
            Quaternion rightStart = bossArenaRightDoor.transform.rotation;

            while (currentLerpTime < lerpTime)
            {
                float t = currentLerpTime / lerpTime;
                bossArenaLeftDoor.transform.rotation = Quaternion.Lerp(leftStart, Quaternion.Euler(leftRotation), t);
                bossArenaRightDoor.transform.rotation = Quaternion.Lerp(rightStart, Quaternion.Euler(rightRotation), t);

                currentLerpTime += Time.deltaTime;
                await Task.Yield(); // Espera o próximo frame
            }

            // Garante a rotação final exata
            bossArenaLeftDoor.transform.rotation = Quaternion.Euler(leftRotation);
            bossArenaRightDoor.transform.rotation = Quaternion.Euler(rightRotation);
        }


        [ServerRpc]
        private void CloseDoorsOnBossFightStartServerRpc(float lerpTime)
        {
            CloseDoorsOnBossFightStartClientRpc(lerpTime);
            CloseDoorsOnBossFightStartClient(lerpTime);
        }

        [ClientRpc]
        private void CloseDoorsOnBossFightStartClientRpc(float lerpTime)
        {
            CloseDoorsOnBossFightStartClient(lerpTime);
        }

        private async void CloseDoorsOnBossFightStartClient(float lerpTime)
        {
            await CloseDoorsLerp(lerpTime);
        }
    }
}
