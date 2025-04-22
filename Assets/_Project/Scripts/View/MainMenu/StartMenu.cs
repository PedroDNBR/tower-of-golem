using Steamworks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

        private void PlaySingleplayer() => SceneManager.LoadScene(1);

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
