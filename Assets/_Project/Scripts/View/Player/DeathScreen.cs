using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TW
{
    public class DeathScreen : NetworkBehaviour
    {
        [Header("Buttons Containers")]
        [SerializeField]
        private Transform deathScreenSingleplayerButtons;

        [SerializeField]
        private Transform deathScreenMultiplayerButtons;

        [Header("Buttons")]
        [SerializeField]
        private Button spectate;

        [SerializeField]
        private Button respawn;

        [SerializeField]
        private Button mainMenu;

        public PlayerNetwork playerNetwork;

        void Start()
        {
            if (NetworkManager.Singleton.IsServer && NetworkManager.Singleton.ConnectedClients.Count == 1)
            {
                deathScreenSingleplayerButtons.gameObject.SetActive(true);
                deathScreenMultiplayerButtons.gameObject.SetActive(false);
            }
            else
            {
                deathScreenSingleplayerButtons.gameObject.SetActive(false);
                deathScreenMultiplayerButtons.gameObject.SetActive(true);
            }

            spectate.onClick.AddListener(Spectate);
            respawn.onClick.AddListener(Respawn);
            mainMenu.onClick.AddListener(ReturnToMainMenu);
        }

        private void Respawn()
        {
            NetworkManager.Singleton.SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

        private void ReturnToMainMenu()
        {
            NetworkManager.Singleton.Shutdown();
            if (GameManager.Instance != null) GameManager.Instance.QuitToMainMenuAndDestroyNetworkManager();
        }

        private void Spectate()
        {
            playerNetwork.HandlePlayerDestroyAndSpectator();
        }
    }
}
