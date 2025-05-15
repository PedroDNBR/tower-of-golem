using TW;
using UnityEngine;
using UnityEngine.UI;

public class PauseOptionsMenu : BaseMenu
{
    [SerializeField]
    Button continueButton;

    [SerializeField]
    Button backButton;

    protected override void OnEnable()
    {
        firstSelectedGameObjectUI = continueButton.gameObject;
        shortcutBackButton = backButton;
        base.OnEnable();
    }
}
