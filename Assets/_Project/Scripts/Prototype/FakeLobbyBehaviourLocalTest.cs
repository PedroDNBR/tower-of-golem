using Unity.Netcode;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TW
{
    public class FakeLobbyBehaviourLocalTest : MonoBehaviour
    {
        [SerializeField]
        NetworkGameManager gameManager;

        private void OnEnable()
        {
            gameManager.OnClientConnectedCallback += StartGame;
        }

        public void StartGame(ulong test)
        {
            if(gameManager.IsServer && gameManager.ConnectedClients.Count > 1)
                gameManager.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }
    }
}
