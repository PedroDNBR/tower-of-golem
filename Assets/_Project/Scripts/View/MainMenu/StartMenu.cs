using Steamworks;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;

namespace TW
{
    public class StartMenu : BaseMenu
    {
        [Header("UI Interactables")]
        [SerializeField] private Button singleplayerButton;
        [SerializeField] private Button multiplayerButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;

        private void OnEnable()
        {
            singleplayerButton.onClick.AddListener(PlaySingleplayer);
            quitButton.onClick.AddListener(QuitGame);
            multiplayerButton.onClick.AddListener(OpenMultiplayerMenu);
            settingsButton.onClick.AddListener(OpenSettingsMenu);
        }

        private void PlaySingleplayer()
        {
            Destroy(NetworkManager.Singleton.GetComponent<UnityTransport>());

            NetworkManager.Singleton.NetworkConfig = new NetworkConfig
            {
                NetworkTransport = NetworkManager.Singleton.GetComponent<NetworkManager>().AddComponent<UnityTransport>()
            };

            NetworkManager.Singleton.StartHost();

            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }

        private void OpenMultiplayerMenu()
        {
            graphicsMenuTransform.gameObject.SetActive(false);
            startMenuTransform.gameObject.SetActive(false);
            multiplayerMenuTransform.gameObject.SetActive(true);
        }

        private void OpenSettingsMenu()
        {
            graphicsMenuTransform.gameObject.SetActive(true);
            startMenuTransform.gameObject.SetActive(false);
            multiplayerMenuTransform.gameObject.SetActive(false);
        }

        private void QuitGame() => Application.Quit();

        private void OnDisable()
        {
            singleplayerButton.onClick.RemoveAllListeners();
            multiplayerButton.onClick.RemoveAllListeners();
            settingsButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();
        }
    }
}
