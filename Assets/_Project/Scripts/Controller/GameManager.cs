using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TW
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Já existe um GameManager, mata esse aqui
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject); // Persiste entre cenas
        }

        public void QuitToMainMenuAndDestroyNetworkManager()
        {
            StartCoroutine(QuitToMainMenuAndDestroyNetworkManagerCoroutine());
        }

        private IEnumerator QuitToMainMenuAndDestroyNetworkManagerCoroutine()
        {
            yield return new WaitForSeconds(0.25f); // tempo suficiente pro transport terminar a coroutine
            Debug.Log("QuitToMainMenuAndDestroyNetworkManagerCoroutine");
            Destroy(NetworkManager.Singleton.gameObject);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
