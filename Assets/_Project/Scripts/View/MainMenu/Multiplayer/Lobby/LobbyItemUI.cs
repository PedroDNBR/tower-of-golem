using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

namespace TW
{
    public class LobbyItemUI : Button
    {
        public TextMeshProUGUI LobbyName;
        public TextMeshProUGUI PlayerCount;

        public Action OnSelectAction;

        protected override void Start() => onClick.AddListener(() => OnSelectAction?.Invoke());
    }
}