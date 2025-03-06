using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : BaseMenu
{
    [SerializeField] private Button backButton;


    private void OnEnable()
    {
        backButton.onClick.AddListener(BackToStartMenu);
    }

    private void BackToStartMenu()
    {
        graphicsMenuTransform.gameObject.SetActive(false);
        startMenuTransform.gameObject.SetActive(true);
    }

}
