using System.Linq;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;
using UnityEngine;

namespace TW
{
    public class SpectatorController : NetworkBehaviour
    {
        private PlayerCamera spectatorCamera;

        [Header("References")]
        public PlayerInput playerInput;

        private Vector2 analogicAim;
        private Vector2 mouseAim;
        private Vector2 lastMouseAim;

        private BaseUI spectatorUI;

        //ulong currentPlayerSpectedId;

        private NetworkVariable<ulong> currentPlayerSpectedId = new NetworkVariable<ulong>(99999);

        private Transform playerToSpectateTransform;

        private void OnEnable()
        {

            if (DialogueMenu.instance != null)
            {
                DialogueMenu.instance.OnDialogStarted += DisableInput;
                DialogueMenu.instance.OnDialogEnded += SetupPlayerInputs;
            }

            SetupPlayerInputs();
        }

        public void Start()
        {
            currentPlayerSpectedId.OnValueChanged += (ulong old, ulong current) =>
            {
                var allPlayers = FindObjectsOfType<PlayerController>();
                foreach (var player in allPlayers)
                {
                    var netObj = player.GetComponentInParent<NetworkObject>();
                    if (netObj != null && netObj.OwnerClientId == current)
                    {
                        if(playerToSpectateTransform != null)
                            playerToSpectateTransform.GetComponent<BaseHealth>().onDeath -= OnPlayerDies;

                        playerToSpectateTransform = player.transform;
                        playerToSpectateTransform.GetComponent<BaseHealth>().onDeath += OnPlayerDies;
                        return;
                    }
                }
            };

            RequestInitialSpectateServerRpc();
        }

        private void OnPlayerDies() => RequestInitialSpectateServerRpc();

        [ServerRpc(RequireOwnership = false)]
        private void RequestInitialSpectateServerRpc(ServerRpcParams rpcParams = default)
        {
            SpectatePlayerByOffset(+1);
            //var aliveKeys = ((NetworkGameManager)NetworkManager.Singleton).players
            //    .Where(p => p.Value != null && p.Value.GetComponentInChildren<PlayerController>() != null)
            //    .Select(p => p.Key)
            //    .OrderBy(k => k)
            //    .ToList();

            //if (aliveKeys.Count == 0) return;

            //ulong clientId = rpcParams.Receive.SenderClientId;
            //var spectator = ((NetworkGameManager)NetworkManager.Singleton).spectators[clientId];
            //var specCtrl = spectator.GetComponentInChildren<SpectatorController>();

            //specCtrl.currentPlayerSpectedId.Value = aliveKeys[0];
        }

        private void SetupPlayerInputs()
        {
            if (playerInput == null)
            {
                playerInput = new PlayerInput();
                playerInput.Actions.PrimarySpell.performed += playerInput => SpectatePlayerByOffsetServerRpc(+1);
                playerInput.Actions.SpecialSpell.performed += playerInput => SpectatePlayerByOffsetServerRpc(-1);
                playerInput.Settings.Menu.performed += playerInput => TogglePauseMenu();
                playerInput.Actions.AnalogicAim.performed += playerInput => analogicAim = playerInput.ReadValue<Vector2>();
                playerInput.Actions.MouseAim.performed += playerInput => mouseAim = playerInput.ReadValue<Vector2>();
                playerInput.Actions.ResetCamera.performed += playerInput => spectatorCamera.ResetCamera();
            }
            playerInput.Enable();
        }

        [ServerRpc]
        private void SpectatePlayerByOffsetServerRpc(int offset)
        {
            SpectatePlayerByOffset(offset);
        }

        private void SpectatePlayerByOffset(int offset)
        {
            var aliveKeys = ((NetworkGameManager)NetworkManager.Singleton).players
                .Where(p => p.Value != null && p.Value.GetComponentInChildren<PlayerController>() != null && p.Value.GetComponentInChildren<BaseHealth>().health.Value > 0)
                .Select(p => p.Key)
                .OrderBy(k => k)
                .ToList();

            if (aliveKeys.Count == 0) return;

            int currentIndex = aliveKeys.IndexOf(currentPlayerSpectedId.Value);

            if (currentIndex == -1)
            {
                currentPlayerSpectedId.Value = aliveKeys[0];
                return;
            }

            int nextIndex = (currentIndex + offset + aliveKeys.Count) % aliveKeys.Count;
            currentPlayerSpectedId.Value = aliveKeys[nextIndex];
        }

        private void TogglePauseMenu()
        {
            spectatorUI.TogglePauseMenu();
        }

        private void OnDisable()
        {
            DisableInput();
            if (DialogueMenu.instance != null)
            {
                DialogueMenu.instance.OnDialogStarted -= DisableInput;
                DialogueMenu.instance.OnDialogEnded -= SetupPlayerInputs;
            }
        }

        private void DisableInput()
        {
            Debug.Log("DisableInput");
            if (playerInput != null) playerInput.Disable();
        }

        public void Awake()
        {
            spectatorCamera = GetComponent<PlayerCamera>();
            spectatorUI = GetComponentInParent<BaseUI>();
        }

        bool usingAnalogic = false;
        bool usingMouse = true;

        private void LateUpdate()
        {
            if (playerInput == null) return;

            if (playerToSpectateTransform != null)
            {
                transform.position = playerToSpectateTransform.position;
            }

            float analogAmout = Mathf.Clamp01(Mathf.Abs(analogicAim.x) + Mathf.Abs(analogicAim.y));
            if (analogAmout > 0 && !usingAnalogic)
            {
                usingAnalogic = true;
                usingMouse = false;
            }
            else if (mouseAim != lastMouseAim)
            {
                usingAnalogic = false;
                usingMouse = true;
            }

            lastMouseAim = mouseAim;

            spectatorCamera.MoveCamera(mouseAim);
        }

        public void Die()
        {
            DisableInput();
            Destroy(spectatorCamera);
            Destroy(this);
        }
    }
}
