using UnityEngine;
using UnityEngine.UI;

namespace TW
{
    public class UiUtils : MonoBehaviour
    {
        public UiErrorMessage uiErrorMessage;

        public static UiUtils Instance;

        public UiUtils() => Instance = this;

        public bool CheckIfStringIsNotEmpty(string text) => string.IsNullOrEmpty(text);

        public UiErrorMessage SetErrorMessage(Transform parent, string errorMessage)
        {
            UiErrorMessage lobbyNameError = Instantiate(uiErrorMessage, parent.position, Quaternion.identity);
            lobbyNameError.transform.SetParent(parent.parent);
            lobbyNameError.transform.SetSiblingIndex(2);
            lobbyNameError.errorMessage.text = errorMessage;
            lobbyNameError.image.sprite = parent.GetComponent<Image>().sprite;
            lobbyNameError.transform.position = parent.position;
            lobbyNameError.transform.rotation = parent.rotation;
            lobbyNameError.transform.localScale = parent.localScale;
            lobbyNameError.name = errorMessage;

            return lobbyNameError;
        }
    }
}
