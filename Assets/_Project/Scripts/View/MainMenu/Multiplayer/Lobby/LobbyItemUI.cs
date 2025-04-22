using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace TW
{
    public class LobbyItemUI : Selectable
    {
        public TextMeshProUGUI LobbyName;
        public TextMeshProUGUI PlayerCount;

        public Action OnSelectAction;

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            OnSelectAction?.Invoke();
        }

    }
}