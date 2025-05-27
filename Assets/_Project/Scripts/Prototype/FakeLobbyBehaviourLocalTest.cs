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

        private void OnEnable()
        {
            networkGameManager.OnClientConnectedCallback += StartGame;
        }

        public void StartGame(ulong test)
        {
            if(networkGameManager.IsServer && networkGameManager.ConnectedClients.Count >= 1)
                networkGameManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
    }
}
