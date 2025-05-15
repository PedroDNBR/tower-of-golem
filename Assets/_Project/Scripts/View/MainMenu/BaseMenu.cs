using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TW
{
    public class BaseMenu : MonoBehaviour
    {
        [Header("UI Transforms")]
        [SerializeField] protected RectTransform startMenuTransform;
        [SerializeField] protected RectTransform graphicsMenuTransform;
        [SerializeField] protected RectTransform multiplayerMenuTransform;

        protected UIInput UiInput;

        protected GameObject firstSelectedGameObjectUI;

        protected Button shortcutBackButton;


        protected virtual void OnEnable()
        {
            if (UiInput == null)
            {
                UiInput = new UIInput();
                UiInput.UI.NavigationKeys.performed += playerInput => SetEventSystemFirstSelected();
                UiInput.UI.BackButton.performed += playerInput => { if (shortcutBackButton != null) shortcutBackButton.onClick.Invoke(); };
            }
            UiInput.Enable();
        }

        protected virtual void OnDisable()
        {
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
            if (UiInput != null)
            {
                UiInput.Disable();
                UiInput.Dispose();
                UiInput = null;
            }
        }

        protected virtual void SetEventSystemFirstSelected() {
            if (EventSystem.current.currentSelectedGameObject == null || !EventSystem.current.currentSelectedGameObject.activeInHierarchy)
            {
                EventSystem.current.SetSelectedGameObject(firstSelectedGameObjectUI);
            }
        }
    }
}