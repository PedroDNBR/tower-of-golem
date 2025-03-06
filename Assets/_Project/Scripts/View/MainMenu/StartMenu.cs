using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : BaseMenu
{
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        singleplayerButton.onClick.AddListener(PlaySingleplayer);
        quitButton.onClick.AddListener(QuitGame);
        settingsButton.onClick.AddListener(OpenSettingsMenu);
    }

    private void PlaySingleplayer() => SceneManager.LoadScene(1);

    void OpenSettingsMenu()
    {
        graphicsMenuTransform.gameObject.SetActive(true);
        startMenuTransform.gameObject.SetActive(false);
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
