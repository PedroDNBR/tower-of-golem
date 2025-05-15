using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

namespace TW
{
    public class StartMenu : BaseMenu
    {
        [Header("UI Interactables")]
        [SerializeField] private Button singleplayerButton;
        [SerializeField] private Button multiplayerButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button quitButton;


        protected override void OnEnable()
        {
            singleplayerButton.onClick.AddListener(PlaySingleplayer);
            quitButton.onClick.AddListener(QuitGame);
            multiplayerButton.onClick.AddListener(OpenMultiplayerMenu);
            settingsButton.onClick.AddListener(OpenSettingsMenu);

            firstSelectedGameObjectUI = singleplayerButton.gameObject;

            base.OnEnable();
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

        protected override void OnDisable()
        {
            singleplayerButton.onClick.RemoveAllListeners();
            multiplayerButton.onClick.RemoveAllListeners();
            settingsButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();

            base.OnDisable();
        }
    }
}
