using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TW
{
    public class FakeLobbyBehaviourLocalTest : MonoBehaviour
    {
        [SerializeField]
        NetworkGameManager networkGameManager;

        [SerializeField]
        int minplayerCount = 1;

        private void OnEnable()
        {
            networkGameManager.OnClientConnectedCallback += CheckStartGame;
        }

        public void CheckStartGame(ulong test)
        {
            if (networkGameManager.IsServer && networkGameManager.ConnectedClients.Count >= minplayerCount)
                StartGame();
        }

        public void StartGame()
        {
            networkGameManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
    }

    [CustomEditor(typeof(FakeLobbyBehaviourLocalTest))]
    public class FakeLobbyBehaviourLocalTestEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector(); // Draw default inspector elements

            FakeLobbyBehaviourLocalTest myScript = (FakeLobbyBehaviourLocalTest)target;

            if (GUILayout.Button("Start"))
            {
                myScript.StartGame();
            }
        }
    }
}
