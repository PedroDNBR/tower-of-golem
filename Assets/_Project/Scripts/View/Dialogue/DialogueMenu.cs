using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TW
{
    public class DialogueMenu : NetworkBehaviour
    {
        public static DialogueMenu instance;

        public DialogueMenu() => instance = this;

        public Action OnDialogStarted;
        public Action OnDialogEnded;

        [Header("UI")]
        public TMP_Text title;
        public TMP_Text dialogue;
        public Button NextButton;

        [Header("System")]
        [SerializeField]
        public List<DialogueData> dialogues = new List<DialogueData>();
    
        public NetworkVariable<int> currentIndex = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private void OnEnable() => OnDialogStarted?.Invoke();

        private void OnDisable() => OnDialogEnded?.Invoke();

        private void Start()
        {
            NextButton.gameObject.SetActive(NetworkManager.Singleton.IsHost);
            currentIndex.OnValueChanged += (int previousValue, int newValue) =>
            {
                NextDialogue(newValue);
            };
            if (NetworkManager.Singleton.IsServer) currentIndex.Value = 0;
            NextButton.onClick.AddListener(() => { currentIndex.Value++; });
            EventSystem.current.SetSelectedGameObject(NextButton.gameObject);
        }

        protected void NextDialogue(int index)
        {
            if (index >= dialogues.Count)
            {
                gameObject.SetActive(false);
                return;
            }
            SetUI(dialogues[index]);
        }

        protected void SetUI(DialogueData data)
        {
            title.text = data.character.characterName;
            dialogue.text = DialogueDatabase.Dialogues[data.dialogueIndex];
        }
    }

    [Serializable]
    public class DialogueData
    {
        public Character character;
        public string dialogueIndex;
    }
}
