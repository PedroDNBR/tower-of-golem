using UnityEngine;

namespace TW
{
    public class BaseMenu : MonoBehaviour
    {
        [Header("UI Transforms")]
        [SerializeField] protected RectTransform startMenuTransform;
        [SerializeField] protected RectTransform graphicsMenuTransform;
        [SerializeField] protected RectTransform multiplayerMenuTransform;
    }
}