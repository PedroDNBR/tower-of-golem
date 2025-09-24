using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TW
{
    public class BaseUI : MonoBehaviour
    {
        [SerializeField]
        protected Canvas canvas;

        [Header("UI Screens")]
        [SerializeField]
        protected Transform pauseMenu;

        [SerializeField]
        protected Transform optionsMenu;

        [SerializeField]
        protected Transform settingsMenu;

        [Header("Menu Buttons")]
        [SerializeField]
        protected Button continueButton;

        [SerializeField]
        protected Button settingsButton;

        [SerializeField]
        protected Button quitButton;

        protected bool quitted = false;

        protected virtual void Start()
        {
            canvas.gameObject.SetActive(true);
            settingsButton.onClick.AddListener(OpenSettings);
            continueButton.onClick.AddListener(ClosePauseMenu);
            quitButton.onClick.AddListener(QuitToMenu);
            NetworkManager.Singleton.OnClientDisconnectCallback += QuitWhenServerDisconnects;
        }

        public void TogglePauseMenu()
        {
            if (pauseMenu.gameObject.activeSelf)
                EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
            else
            {
                settingsMenu.gameObject.SetActive(false);
                optionsMenu.gameObject.SetActive(true);
            }

            pauseMenu.gameObject.SetActive(!pauseMenu.gameObject.activeSelf);
        }

        protected bool GetPauseMenuIsOpen() => pauseMenu.gameObject.activeSelf;

        public void OpenSettings()
        {
            optionsMenu.gameObject.SetActive(false);
            settingsMenu.gameObject.SetActive(true);
        }

        protected void ClosePauseMenu()
        {
            pauseMenu.gameObject.SetActive(false);
            optionsMenu.gameObject.SetActive(true);
            settingsMenu.gameObject.SetActive(false);
        }

        protected void QuitToMenu()
        {
            quitted = true;
            NetworkManager.Singleton.Shutdown();
            if (GameManager.Instance != null) GameManager.Instance.QuitToMainMenuAndDestroyNetworkManager();
        }

        protected void QuitWhenServerDisconnects(ulong id)
        {
            if ((id == 0 || id == NetworkManager.Singleton.LocalClientId) && !quitted)
            {
                QuitToMenu();
            }
        }
    }
}
