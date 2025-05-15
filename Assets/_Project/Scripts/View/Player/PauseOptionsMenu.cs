using TW;
using UnityEngine;
using UnityEngine.UI;

public class PauseOptionsMenu : BaseMenu
{
    [SerializeField]
    Button continueButton;

    protected override void OnEnable()
    {
        firstSelectedGameObjectUI = continueButton.gameObject;

        base.OnEnable();
    }
}
