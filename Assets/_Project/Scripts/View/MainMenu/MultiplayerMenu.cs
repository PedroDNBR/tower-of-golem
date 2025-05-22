using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Steamworks;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using Netcode.Transports;
using System.Collections;

namespace TW
{
    public class MultiplayerMenu : BaseMenu
    {
        [SerializeField] protected RectTransform lobbyMenuTransform;
        [SerializeField] protected RectTransform hostModalTransform;
        [SerializeField] protected RectTransform hostMenuTransform;
        [SerializeField] protected RectTransform playerListTransform;
        [SerializeField] protected RectTransform lobbyListTransform;
        [SerializeField] protected RectTransform joinModalTransform;

        [Header("UI Prefabs")]
        [SerializeField] protected LobbyItemUI lobbyListItemPrefab;
        [SerializeField] protected PlayerItemUI playerListItemPrefab;

        [Header("UI Buttons")]
        [SerializeField] private Button refreshLobbyListButton;
        [SerializeField] private Button lobbyBackButton;
        [SerializeField] private Button hostModalBackButton;
        [SerializeField] private Button hostMenuBackButton;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button createLobbyButton;
        [SerializeField] private Button startButton;
        [SerializeField] private Button joinButton;
        [SerializeField] private Button backJoinModalButton;
        [SerializeField] private Button joinModalButton;

        [Header("UI Inputs")]
        [SerializeField] private TMP_InputField lobbyNameInput;
        private UiErrorMessage lobbyNameInputError;
        [SerializeField] private TMP_InputField lobbyPasswordInput;
        private UiErrorMessage lobbyPasswordInputError;
        [SerializeField] private TMP_InputField lobbyModalPasswordInput;
        private UiErrorMessage lobbyModalPasswordInputError;

        [Header("UI Text")]
        [SerializeField] private TMP_Text lobbyNameText;

        [Header("Lobby data")]
        public CSteamID currentLobbyId;
        private const string HostAddressKey = "HostAddress";

        protected Callback<LobbyCreated_t> LobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> JoinRequest;
        protected Callback<LobbyEnter_t> LobbyEntered;
        protected Callback<LobbyInvite_t> LobbyInvite;
        protected Callback<LobbyChatUpdate_t> LobbyChatUpdate;

        protected Callback<LobbyMatchList_t> LobbyList;
        protected Callback<LobbyDataUpdate_t> LobbyDataUpdate;

        public List<CSteamID> lobbiesId = new List<CSteamID>();


        private Callback<GetTicketForWebApiResponse_t> AuthTicketForWebApiResponseCallback;
        private string ticket;
        private string identity = "unityauthenticationservice";

        private const int MAX_PLAYER_IN_LOBBY = 4;

        private bool hasTriedToStartClient = false;

        protected override void OnEnable()
        {
            SetButtonListeners();
            SetInputListeners();
            SetEventSystemNavigationToLobbyList();

            base.OnEnable();

            SetSteamworksCallbacks();

            QueryLobbies();
        }

        private void SetEventSystemNavigationToLobbyList()
        {
            firstSelectedGameObjectUI = refreshLobbyListButton.gameObject;
            shortcutBackButton = lobbyBackButton;
        }

        private void SetInputListeners()
        {
            lobbyPasswordInput.onValueChanged.AddListener((string value) =>
            {
                ValidatePassword(value, ref lobbyPasswordInputError);
            });

            lobbyNameInput.onValueChanged.AddListener((string value) =>
            {
                if (String.IsNullOrEmpty(value) || lobbyNameInputError == null) return;
                Destroy(lobbyNameInputError.gameObject);
                lobbyNameInputError = null;
            });

            lobbyModalPasswordInput.onValueChanged.AddListener((string value) =>
            {
                ValidatePassword(value, ref lobbyModalPasswordInputError);
            });
        }

        void ValidatePassword(string value, ref UiErrorMessage errorMessage)
        {
            if (errorMessage == null) return;

            if (!String.IsNullOrEmpty(value))
            {
                if (value.Length < 8 || value.Length > 64)
                {
                    return;
                }
            }
            Destroy(errorMessage.gameObject);
            errorMessage = null;
        }

        private void SetButtonListeners()
        {
            lobbyBackButton.onClick.AddListener(BackToStartMenu);
            hostButton.onClick.AddListener(() => HostNewGame(true));
            hostModalBackButton.onClick.AddListener(() => HostNewGame(false));
            createLobbyButton.onClick.AddListener(CreateLobby);
            hostMenuBackButton.onClick.AddListener(BackFromLobby);
            startButton.onClick.AddListener(StartGame);
            refreshLobbyListButton.onClick.AddListener(RefreshLobbyList);
            backJoinModalButton.onClick.AddListener(BackFromJoinModal);
        }

        private void SetSteamworksCallbacks()
        {
            LobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            JoinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
            LobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
            LobbyInvite = Callback<LobbyInvite_t>.Create(OnLobbyInvite);

            LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);

            LobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbyList);
            LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
        }

        #region LobbyCreation

        private void HostNewGame(bool isActive)
        {
            hostModalTransform.gameObject.SetActive(isActive);
            if(isActive)
            {
                EventSystem.current.SetSelectedGameObject(lobbyNameInput.gameObject);
                shortcutBackButton = hostModalBackButton;
            }
            else
            {
                SetEventSystemNavigationToLobbyList();
            }
        }

        private void CreateLobby()
        {
            if (SteamManager.Initialized)
            {
                if (String.IsNullOrEmpty(lobbyNameInput.text))
                {
                    SetErrorInLobbyCreation(lobbyNameInput.transform, ref lobbyNameInputError, "Lobby name is required");
                    return;
                }

                if (!ValidatePasswordCreateLobby(ref lobbyPasswordInput, ref lobbyPasswordInputError)) return;

                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 4);
            }
        }

        private void OnLobbyCreated(LobbyCreated_t callback)
        {
            Debug.Log(callback.m_eResult);
            if (callback.m_eResult != EResult.k_EResultOK) return;

            Debug.Log($"createLobbyButton {createLobbyButton}");
            createLobbyButton.interactable = false;
            hostModalBackButton.interactable = false;
            string lobbyName = lobbyNameInput.text;
            string lobbyPassword = lobbyPasswordInput.text;

            CSteamID steamID = new CSteamID(callback.m_ulSteamIDLobby);

            SteamMatchmaking.SetLobbyData(steamID, HostAddressKey, SteamUser.GetSteamID().ToString());
            SteamMatchmaking.SetLobbyData(steamID, "name", lobbyName);
            if(!String.IsNullOrEmpty(lobbyPassword)) SteamMatchmaking.SetLobbyData(steamID, "password", lobbyPassword);
            SteamMatchmaking.SetLobbyData(steamID, "identifier", "feijaocomarroz");

            createLobbyButton.interactable = true;
            hostModalBackButton.interactable = true;

            QueryLobbies();
        }

        private bool ValidatePassword(ref TMP_InputField passwordInput, ref UiErrorMessage passwordInputError)
        {
            if (!String.IsNullOrEmpty(passwordInput.text))
            {
                if (passwordInput.text.Length < 3 || passwordInput.text.Length > 64)
                {
                    SetErrorInLobbyCreation(passwordInput.transform, ref passwordInputError, "Password must have between 3 and 64 characters");
                    return false;
                }
                return true;
            }
            SetErrorInLobbyCreation(passwordInput.transform, ref passwordInputError, "Password can't be empty");
            return false;
        }

        private bool ValidatePasswordCreateLobby(ref TMP_InputField passwordInput, ref UiErrorMessage passwordInputError)
        {
            if (!String.IsNullOrEmpty(passwordInput.text))
            {
                if (passwordInput.text.Length < 3 || passwordInput.text.Length > 64)
                {
                    SetErrorInLobbyCreation(passwordInput.transform, ref passwordInputError, "Password must have between 3 and 64 characters");
                    return false;
                }
                return true;
            }
            return true;
        }

        private void SetErrorInLobbyCreation(Transform parentTransform, ref UiErrorMessage errorMessage, string errorText)
        {
            createLobbyButton.interactable = true;
            hostModalBackButton.interactable = true;
            joinModalButton.interactable = true;
            if (errorMessage != null && errorMessage.name == errorText) return;
            if (errorMessage != null) Destroy(errorMessage.gameObject);
            errorMessage = UiUtils.Instance.SetErrorMessage(parentTransform.transform, errorText);
            return;
        }

        private void LoadLobbyUI()
        {
            SetNewLobbyUI();
            LoadPlayerListUI();
            LoadLobbyNameUI();
            StartClient();
            CheckIfIsHost();
        }

        private void SetNewLobbyUI()
        {
#if !UNITY_EDITOR
            if (!SteamManager.Initialized) return;
#endif
            lobbyMenuTransform.gameObject.SetActive(false);
            hostModalTransform.gameObject.SetActive(false);
            hostMenuTransform.gameObject.SetActive(true);
            shortcutBackButton = hostMenuBackButton;
            EventSystem.current.SetSelectedGameObject(hostMenuBackButton.gameObject);

            lobbyPasswordInput.text = lobbyNameInput.text = String.Empty;
            joinButton.onClick.RemoveAllListeners();
            joinButton.interactable = false;
        }

#endregion

        #region LobbyRetrieval

        List<CSteamID> lobbyList = new List<CSteamID>();

        private void OnGetLobbyList(LobbyMatchList_t result)
        {
            if (!SteamManager.Initialized) return;
            lobbyList = new List<CSteamID>();
            for (int i = 0; i < result.m_nLobbiesMatching; i++)
            {
                CSteamID lobbyId = SteamMatchmaking.GetLobbyByIndex(i);
                lobbyList.Add(lobbyId);
                SteamMatchmaking.RequestLobbyData(lobbyId);
            }
        }

        private void OnGetLobbyData(LobbyDataUpdate_t result)
        {
            DisplayLobbies(lobbyList, result);
            if (currentLobbyId != null && !IsLobbyHost() && currentLobbyId == new CSteamID(result.m_ulSteamIDLobby))
                StartClient();
        }

        private void DisplayLobbies(List<CSteamID> lobbyIds, LobbyDataUpdate_t result)
        {
            if (!SteamManager.Initialized) return;

            if (lobbyListTransform == null) return;
            foreach (Transform child in lobbyListTransform.transform)
                Destroy(child.gameObject);

            for (int i = 0; i < lobbyIds.Count; i++)
            {
                if (lobbyIds[i].m_SteamID == result.m_ulSteamIDLobby)
                {
                    GameObject lobbyItem = Instantiate(lobbyListItemPrefab.gameObject, lobbyListTransform);
                    LobbyItemUI lobbyItemUI = lobbyItem.GetComponent<LobbyItemUI>();
                    CSteamID lobbyId = (CSteamID)lobbyIds[i].m_SteamID;

                    lobbyItemUI.LobbyName.text = SteamMatchmaking.GetLobbyData(lobbyId, "name");
                    lobbyItemUI.PlayerCount.text = $"{SteamMatchmaking.GetNumLobbyMembers(lobbyId)}/4";
                    lobbyItemUI.OnSelectAction += () => SetupLobbyToJoin(lobbyId);
                }
            }
        }

        private void SetupLobbyToJoin(CSteamID lobbyId)
        {
            joinButton.interactable = true;
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(() =>
            {
                joinButton.interactable = false;
                if (!String.IsNullOrEmpty(SteamMatchmaking.GetLobbyData(lobbyId, "password")))
                {
                    OpenJoinPasswordModal(lobbyId);
                }
                else
                {
                    JoinLobbyById(lobbyId);
                }
                hasTriedToStartClient = false;
            });
                        EventSystem.current.SetSelectedGameObject(joinButton.gameObject);

        }

        private void QueryLobbies()
        {
            if (!SteamManager.Initialized) return;

            SteamMatchmaking.AddRequestLobbyListStringFilter("identifier", "feijaocomarroz", ELobbyComparison.k_ELobbyComparisonEqual);
            SteamMatchmaking.RequestLobbyList();
        }

        private void RefreshLobbyList()
        {
            refreshLobbyListButton.interactable = false;
            try
            {
                QueryLobbies();
            }
            catch (Exception ex)
            {
            }
            refreshLobbyListButton.interactable = true;
        }

        #endregion

        #region LobbyPage

        private void LoadPlayerListUI()
        {
            if(!SteamManager.Initialized) return;
            if (currentLobbyId == null) return;
            CSteamID LobbyId = currentLobbyId;

            Debug.Log("Carregando lista de jogadores");

            foreach (Transform item in playerListTransform)
                Destroy(item.gameObject);

            for (int i = 0; i < SteamMatchmaking.GetNumLobbyMembers(LobbyId); i++)
            {
                AddPlayerToLobbyPlayerList(SteamMatchmaking.GetLobbyMemberByIndex(LobbyId, i));
            }
        }

        private void LoadLobbyNameUI() => lobbyNameText.text = SteamMatchmaking.GetLobbyData(currentLobbyId, "name");

        private void AddPlayerToLobbyPlayerList(CSteamID memberId)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab.gameObject, playerListTransform);
            PlayerItemUI playerItemUI = playerItem.GetComponent<PlayerItemUI>();
            playerItemUI.PlayerNameText.text = SteamFriends.GetFriendPersonaName(memberId);
        }

        #endregion

        #region JoinLobby

        private void OnJoinRequest(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        private void OnLobbyEntered(LobbyEnter_t callback)
        {
            // Everyone
            currentLobbyId = new CSteamID(callback.m_ulSteamIDLobby);
            LoadLobbyUI();

            if (IsLobbyHost())
            {
                // Host
                startButton.gameObject.SetActive(true);
            }
            else
            {
                // Clients
                startButton.gameObject.SetActive(false);
                StartCoroutine(WaitForServerStart());
            }
        }

        private void OnLobbyChatUpdate(LobbyChatUpdate_t callback)
        {
            LoadLobbyUI();
        }

        private void OnLobbyInvite(LobbyInvite_t callback)
        {
            SteamMatchmaking.JoinLobby(new CSteamID(callback.m_ulSteamIDLobby));
        }

        private void BackFromLobby()
        {
            CloseLobby();
            lobbyMenuTransform.gameObject.SetActive(true);
            hostModalTransform.gameObject.SetActive(false);
            hostMenuTransform.gameObject.SetActive(false);
            SetEventSystemNavigationToLobbyList();
        }

        private void BackFromJoinModal()
        {
            joinModalTransform.gameObject.SetActive(false);
            lobbyModalPasswordInput.text = String.Empty;
            joinButton.interactable = true;
            SetEventSystemNavigationToLobbyList();
        }

        private bool JoinLobbyById(CSteamID lobbyId, string password = "")
        {
            startButton.gameObject.SetActive(false);
            if (String.IsNullOrEmpty(password))
            {
                if (SteamManager.Initialized)
                    SteamMatchmaking.JoinLobby(lobbyId);
            }
            else
            {
                if(SteamMatchmaking.GetLobbyData(lobbyId, "password") == password)
                {
                    SteamMatchmaking.JoinLobby(lobbyId);
                }
                else
                {
                    SetErrorInLobbyCreation(lobbyModalPasswordInput.transform, ref lobbyModalPasswordInputError, "Password is incorrect");
                    return false;
                }
            }
            EventSystem.current.SetSelectedGameObject(hostMenuBackButton.gameObject);
            shortcutBackButton = hostMenuBackButton;
            return true;
        }
        
        private void OpenJoinPasswordModal(CSteamID lobbyId)
        {
            joinModalTransform.gameObject.SetActive(true);
            joinModalButton.onClick.AddListener(() => JoinLobbyViaModal(lobbyId));
            EventSystem.current.SetSelectedGameObject(lobbyModalPasswordInput.gameObject);
            shortcutBackButton = backJoinModalButton;
        }

        private void JoinLobbyViaModal(CSteamID lobbyId)
        {
            if (!ValidatePassword(ref lobbyModalPasswordInput, ref lobbyModalPasswordInputError)) return;
            if (JoinLobbyById(lobbyId, lobbyModalPasswordInput.text))
            {
                joinModalTransform.gameObject.SetActive(false);
                lobbyModalPasswordInput.text = String.Empty;
                if (lobbyModalPasswordInputError != null && lobbyModalPasswordInputError.gameObject != null) Destroy(lobbyModalPasswordInputError.gameObject);
                lobbyModalPasswordInputError = null;
            }
        }

        #endregion

        private void BackToStartMenu()
        {
            startMenuTransform.gameObject.SetActive(true);
            graphicsMenuTransform.gameObject.SetActive(false);
            multiplayerMenuTransform.gameObject.SetActive(false);
        }

        private bool IsLobbyHost()
        {
            return currentLobbyId != null && SteamMatchmaking.GetLobbyOwner(currentLobbyId) == SteamUser.GetSteamID();
        }

        private void CloseLobby()
        {
            if (!SteamManager.Initialized || currentLobbyId == null) return;
            SteamMatchmaking.LeaveLobby(currentLobbyId);
            currentLobbyId = new CSteamID(0);
        }

        private void StartGame()
        {
            if (currentLobbyId == null) return;
            startButton.interactable = false;
            Debug.Log("ConnectToSteamID = ((ulong)SteamMatchmaking.GetLobbyOwner(currentLobbyId)");
            NetworkManager.Singleton.GetComponent<SteamNetworkingSocketsTransport>().ConnectToSteamID = ((ulong)SteamMatchmaking.GetLobbyOwner(currentLobbyId));
            Debug.Log("SetLobbyData(currentLobbyId, \"start_server\", \"true\")");
            SteamMatchmaking.SetLobbyData(currentLobbyId, "start_server", "true");
            Debug.Log("StartHost()");
            NetworkManager.Singleton.StartHost();

            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += (ulong test) => {
                    ChangeSceneToGameScene();
                };
                ChangeSceneToGameScene();
            }
        }

        public void StartClient()
        {
            Debug.Log($"hasTriedToStartClient {hasTriedToStartClient}");
            if (currentLobbyId == null || hasTriedToStartClient) return;
            if (
                !IsLobbyHost() && 
                SteamMatchmaking.GetLobbyData(currentLobbyId, "start_server") == "true"
                )
            {
                hasTriedToStartClient = true;
                NetworkManager.Singleton.GetComponent<SteamNetworkingSocketsTransport>().ConnectToSteamID = ((ulong)SteamMatchmaking.GetLobbyOwner(currentLobbyId));
                NetworkManager.Singleton.StartClient();
            }
        }

        private void ChangeSceneToGameScene()
        {
            if (currentLobbyId == null) return;
            if (NetworkManager.Singleton.ConnectedClientsList.Count != SteamMatchmaking.GetNumLobbyMembers(currentLobbyId)) return;

            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }

        private void CheckIfIsHost()
        {
            if (currentLobbyId == null) return;
            startButton.gameObject.SetActive(IsLobbyHost());
        }

        IEnumerator WaitForServerStart()
        {
            while (true)
            {
                if (currentLobbyId == null) yield break;
                string startServer = SteamMatchmaking.GetLobbyData(currentLobbyId, "start_server");

                if (startServer == "true")
                {
                    StartClient();
                    yield break;
                }

                yield return new WaitForSeconds(1f); // verifica a cada segundo
            }
        }

        protected override void OnDisable()
        {
            UnsetButtonListeners();
            DisposeSteamworksCallbacks();
            base.OnDisable();
        }

        private void UnsetButtonListeners()
        {
            lobbyBackButton.onClick.RemoveAllListeners();
            hostButton.onClick.RemoveAllListeners();
            hostModalBackButton.onClick.RemoveAllListeners();
            createLobbyButton.onClick.RemoveAllListeners();
            hostMenuBackButton.onClick.RemoveAllListeners();
            startButton.onClick.RemoveAllListeners();
            refreshLobbyListButton.onClick.RemoveAllListeners();
            backJoinModalButton.onClick.RemoveAllListeners();
        }

        private void DisposeSteamworksCallbacks()
        {
            LobbyCreated?.Dispose();
            JoinRequest?.Dispose();
            LobbyEntered?.Dispose();
            LobbyInvite?.Dispose();

            LobbyChatUpdate?.Dispose();

            LobbyList?.Dispose();
            LobbyDataUpdate?.Dispose();
        }

    }
}

