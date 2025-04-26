using UnityEngine;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;
using Steamworks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine.SceneManagement;

namespace TW
{
    public class MultiplayerMenu : BaseMenu
    {
        [SerializeField] protected RectTransform lobbyMenuTransform;
        [SerializeField] protected RectTransform hostModalTransform;
        [SerializeField] protected RectTransform hostMenuTransform;
        [SerializeField] protected RectTransform lobbyListTransform;
        [SerializeField] protected RectTransform playerListTransform;

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

        [Header("UI Inputs")]
        [SerializeField] private TMP_InputField lobbyNameInput;
        [SerializeField] private TMP_InputField lobbyPasswordInput;

        [Header("UI Text")]
        [SerializeField] private TMP_Text lobbyNameText;


        [Header("Lobby data")]
        public Lobby currentLobby;

        private Callback<GetTicketForWebApiResponse_t> AuthTicketForWebApiResponseCallback;
        private string ticket;
        private string identity = "unityauthenticationservice";

        private ILobbyEvents lobbyEvents;

        private const int MAX_PLAYER_IN_LOBBY = 4;

        private async void OnEnable()
        {
            lobbyBackButton.onClick.AddListener(BackToStartMenu);
            hostButton.onClick.AddListener(() => HostNewGame(true));
            hostModalBackButton.onClick.AddListener(() => HostNewGame(false));
            createLobbyButton.onClick.AddListener(CreateLobby);
            hostMenuBackButton.onClick.AddListener(BackFromLobby);
            startButton.onClick.AddListener(StartGame);
            refreshLobbyListButton.onClick.AddListener(RefreshLobbyList);

            //await UnityServices.InitializeAsync();

            //AuthTicketForWebApiResponseCallback = Callback<GetTicketForWebApiResponse_t>.Create(OnAuthCallback);

            //SteamUser.GetAuthTicketForWebApi(identity);

            await UnityServices.InitializeAsync();

        #if UNITY_EDITOR
            AuthenticationService.Instance.SignOut(true);
        #endif
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            try
            {
                if (
                    AuthenticationService.Instance == null &&
                    !AuthenticationService.Instance.IsSignedIn
                    )
                {
                    await UnityServices.InitializeAsync();
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            await QueryLobbies();
        }

        async void OnAuthCallback(GetTicketForWebApiResponse_t callback)
        {
            ticket = BitConverter.ToString(callback.m_rgubTicket).Replace("-", string.Empty);
            AuthTicketForWebApiResponseCallback.Dispose();
            AuthTicketForWebApiResponseCallback = null;
            Debug.Log("Steam Login success. Session Ticket: " + ticket);
            await SignInWithSteamAsync(ticket, identity);
            QueryLobbies();
            // Call Unity Authentication SDK to sign in or link with Steam, displayed in the following examples, using the same identity string and the m_SessionTicket.
        }

        async Task SignInWithSteamAsync(string ticket, string identity)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithSteamAsync(ticket, identity);
                Debug.Log("SignIn is successful.");
            }
            catch (AuthenticationException ex)
            {
                // Compare error code to AuthenticationErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
            catch (RequestFailedException ex)
            {
                // Compare error code to CommonErrorCodes
                // Notify the player with the proper error message
                Debug.LogException(ex);
            }
        }

        #region LobbyCreation

        private void HostNewGame(bool isActive) => hostModalTransform.gameObject.SetActive(isActive);

        private async void CreateLobby()
        {
            try
            {
                createLobbyButton.interactable = false;
                hostModalBackButton.interactable = false;
                string lobbyName = lobbyNameInput.text;
                if (String.IsNullOrEmpty(lobbyName)) return;
                CreateLobbyOptions options = new CreateLobbyOptions();
                if (!String.IsNullOrEmpty(lobbyPasswordInput.text))
                {
                    options.IsPrivate = true;
                    options.Password = lobbyPasswordInput.text;
                }
                options.Data = new Dictionary<string, DataObject>()
                {
                    {
                        "GameMode", new DataObject(
                            visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
                            value: "Campaign",
                            index: DataObject.IndexOptions.S1)
                    },
                };

                currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MAX_PLAYER_IN_LOBBY, options);

                StartCoroutine(HeartbeatLobbyCoroutine());

                await LoadLobbyUI();

                startButton.gameObject.SetActive(true);
                createLobbyButton.interactable = true;
                hostModalBackButton.interactable = true;
            }
            catch (LobbyServiceException e)
            {
                Debug.LogException(e);
                createLobbyButton.interactable = true;
                hostModalBackButton.interactable = true;
            }
        }

        private async Task LoadLobbyUI()
        {
            await SetLobbyCallbacks();

            await SendPlayerNameToLobby();

            SetNewLobbyUI();
        }

        private void SetNewLobbyUI()
        {
#if !UNITY_EDITOR
            if (!SteamManager.Initialized) return;
#endif
            lobbyMenuTransform.gameObject.SetActive(false);
            hostModalTransform.gameObject.SetActive(false);
            hostMenuTransform.gameObject.SetActive(true);

            lobbyPasswordInput.text = lobbyNameInput.text = String.Empty;
            joinButton.onClick.RemoveAllListeners();
            joinButton.interactable = false;
        }

        private async Task SetLobbyCallbacks()
        {
            var callbacks = new LobbyEventCallbacks();
            callbacks.LobbyChanged += OnLobbyChanged;
            callbacks.PlayerJoined += OnPlayerJoined;
            callbacks.PlayerLeft += OnPlayerLeft;
            callbacks.KickedFromLobby += BackFromLobby;
            callbacks.LobbyDeleted += BackFromLobby;

            try
            {
                lobbyEvents = await Lobbies.Instance.SubscribeToLobbyEventsAsync(currentLobby.Id, callbacks);
            }
            catch (LobbyServiceException ex)
            {
                switch (ex.Reason)
                {
                    case LobbyExceptionReason.AlreadySubscribedToLobby: Debug.LogWarning($"Already subscribed to lobby[{currentLobby.Id}]. We did not need to try and subscribe again. Exception Message: {ex.Message}"); break;
                    case LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy: Debug.LogError($"Subscription to lobby events was lost while it was busy trying to subscribe. Exception Message: {ex.Message}"); throw;
                    case LobbyExceptionReason.LobbyEventServiceConnectionError: Debug.LogError($"Failed to connect to lobby events. Exception Message: {ex.Message}"); throw;
                    default: throw;
                }
            }
        }

        private void AddLobbyToUIList(Lobby lobby)
        {
            GameObject lobbyItem = Instantiate(lobbyListItemPrefab.gameObject, lobbyListTransform);
            LobbyItemUI lobbyItemUI = lobbyItem.GetComponent<LobbyItemUI>();
            lobbyItemUI.LobbyName.text = lobby.Name;
            lobbyItemUI.PlayerCount.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
            lobbyItemUI.OnSelectAction += () => SetupLobbyToJoin(lobby.Id);
        }

        IEnumerator HeartbeatLobbyCoroutine()
        {
            yield return new WaitForSeconds(5);
            if (currentLobby != null)
            {
                LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
                if (IsLobbyHost())
                {
                    StartCoroutine(HeartbeatLobbyCoroutine());
                }
            }
        }

        async Task Delay() => Thread.Sleep(500);

#endregion

        #region LobbyRetrieval

        private async Task QueryLobbies()
        {
            // Common query options to use for paging
            var queryOptions = new QueryLobbiesOptions
            {
                SampleResults = false, // Paging cannot use randomized results
                Filters = new List<QueryFilter>
                {
                    // Only include open lobbies in the pages
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                },
                Order = new List<QueryOrder>
                {
                    // Show the oldest lobbies first
                    new QueryOrder(true, QueryOrder.FieldOptions.Created),
                }
            };

            var response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
            var lobbies = response.Results;

            foreach (Transform child in lobbyListTransform.transform)
                Destroy(child.gameObject);

            // A continuation token will still be returned when the next page is empty,
            // so continue paging until there are no new lobbies in the response
            while (lobbies.Count > 0)
            {
                Debug.Log("procurando segunda paginas de lobby");
                // Do something here with the lobbies in the current page
                foreach (var lobby in lobbies)
                {
                    AddLobbyToUIList(lobby);
                }

                await Delay();

                // Get the next page. Be careful not to modify the filter or order in the
                // query options, as this will return an error
                queryOptions.ContinuationToken = response.ContinuationToken;
                response = await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
                lobbies = response.Results;
            }
        }

        private async void RefreshLobbyList()
        {
            refreshLobbyListButton.interactable = false;
            try
            {
                await QueryLobbies();
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
            Debug.Log("Carregando lista de jogadores");

            foreach (Transform item in playerListTransform)
                Destroy(item.gameObject);

            foreach (Player player in currentLobby.Players)
            {
                AddPlayerToLobbyPlayerList(player);
            }
        }

        private void LoadLobbyNameUI() => lobbyNameText.text = currentLobby.Name;

        private void AddPlayerToLobbyPlayerList(Player player)
        {
            GameObject playerItem = Instantiate(playerListItemPrefab.gameObject, playerListTransform);
            PlayerItemUI playerItemUI = playerItem.GetComponent<PlayerItemUI>();
            if (player.Data == null) return;
            Debug.Log($"player.Data.Count: {player.Data.Count}");
            foreach (var data in player.Data)
            {
                Debug.Log($"player name on Screen: {data.Value.Value}");
                playerItemUI.PlayerNameText.text = data.Value.Value;
            }

        }

        private async Task SendPlayerNameToLobby()
        {
            try
            {
                string playerName;
#if !UNITY_EDITOR
                if (!SteamManager.Initialized) return;
                Debug.Log("Logado na steam");
                playerName = SteamFriends.GetPersonaName();
#else
                playerName = $"Jogador {UnityEngine.Random.Range(0, 1000)}";

#endif

                UpdatePlayerOptions options = new UpdatePlayerOptions();

                options.Data = new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "PlayerName", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member,
                            value: playerName
                        )
                    }
                };

                string playerId = AuthenticationService.Instance.PlayerId;

                var lobby = await LobbyService.Instance.UpdatePlayerAsync(currentLobby.Id, playerId, options);
                Debug.Log($"Enviou o nome: {playerName}");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

#endregion

        private void BackFromLobby()
        {
            CloseLobby();
            lobbyMenuTransform.gameObject.SetActive(true);
            hostModalTransform.gameObject.SetActive(false);
            hostMenuTransform.gameObject.SetActive(false);
        }

        private void BackToStartMenu()
        {
            CloseLobby();
            startMenuTransform.gameObject.SetActive(true);
            graphicsMenuTransform.gameObject.SetActive(false);
            multiplayerMenuTransform.gameObject.SetActive(false);
        }

        private bool IsLobbyHost()
        {
            return currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId;
        }

        private async void CloseLobby()
        {
            if (currentLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);

                currentLobby = null;
                Debug.Log("Quitting Lobby");
            }
        }

        private void OnPlayerLeft(List<int> playerId)
        {
            LoadPlayerListUI();
            CheckIfIsHost();
        }

        private void OnPlayerJoined(List<LobbyPlayerJoined> players)
        {
            LoadPlayerListUI();
            CheckIfIsHost();
        }

        private void OnLobbyChanged(ILobbyChanges changes)
        {
            if (changes.LobbyDeleted)
            {
                // Handle lobby being deleted
                // Calling changes.ApplyToLobby will log a warning and do nothing
            }
            else
            {
                changes.ApplyToLobby(currentLobby);
                LoadPlayerListUI();
                LoadLobbyNameUI();
                CheckIfShouldStartGame();
                CheckIfIsHost();
            }
        }

        private void SetupLobbyToJoin(string lobbyId)
        {
            Debug.Log($"Clicked on a lobby: {lobbyId}");
            joinButton.interactable = true;
            joinButton.onClick.RemoveAllListeners();
            joinButton.onClick.AddListener(async () =>
            {
                joinButton.interactable = false;
                try
                {
                    startButton.gameObject.SetActive(false);
                    Debug.Log($"Joining Lobby: {lobbyId}");
                    Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
                    currentLobby = joinedLobby;
                    await LoadLobbyUI();
                }
                catch (LobbyServiceException e)
                {
                    joinButton.interactable = true;
                    Debug.Log(e);
                }
            });
        }

        private async void StartGame()
        {
            if (currentLobby == null) return;
            startButton.interactable = false;

            string joinCode = await StartHost();


            UpdateLobbyOptions options = new UpdateLobbyOptions();

            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.OnClientConnectedCallback += (ulong test) => {
                    ChangeSceneToGameScene();
                };
            }

            options.Data = new Dictionary<string, DataObject>()
            {
                {
                    "JoinCode",
                    new DataObject(
                        visibility: DataObject.VisibilityOptions.Member,
                        value: joinCode
                    )
                }
            };

            //currentLobby.Data.Add(
            //    "JoinCode",
            //    new DataObject(
            //        visibility: DataObject.VisibilityOptions.Member,
            //        value: joinCode
            //    )
            //);

            currentLobby = await LobbyService.Instance.UpdateLobbyAsync(currentLobby.Id, options);

            if (NetworkManager.Singleton.IsHost)
                ChangeSceneToGameScene();

        }

        private void CheckIfShouldStartGame()
        {
            if (currentLobby == null) return;
            if (IsLobbyHost()) return;
            if (currentLobby.Data != null && !currentLobby.Data.ContainsKey("JoinCode")) return;
            StartClient();
        }

        private async Task<string> StartHost()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                
                GUIUtility.systemCopyBuffer = joinCode;

                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartHost();

                return joinCode;
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
            }
            return String.Empty;
        }

        public async void StartClient()
        {
            if (currentLobby == null) return;
            if (currentLobby.Data != null && !currentLobby.Data.ContainsKey("JoinCode")) return;
            try
            {
                string joinCode = currentLobby.Data["JoinCode"].Value;
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);

                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.LogError(e);
            }
        }

        private void ChangeSceneToGameScene()
        {
            if (currentLobby == null) return;
            if (NetworkManager.Singleton.ConnectedClientsList.Count != currentLobby.Players.Count) return;

            CloseLobby();
            NetworkManager.Singleton.SceneManager.LoadScene("SampleScene", LoadSceneMode.Single);
        }

        private void CheckIfIsHost()
        {
            if (currentLobby == null) return;
            startButton.gameObject.SetActive(IsLobbyHost());
        }

    }
}

