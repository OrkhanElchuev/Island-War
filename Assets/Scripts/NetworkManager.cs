using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    // Public  

    [Header("Login UI Panel")]
    public InputField playerNameInput;
    public GameObject loginPanel;

    [Header("Connection Status")]
    public Text connectionStatusText;

    [Header("Game Options UI Panel")]
    public GameObject gameOptionsPanel;

    [Header("Create Room UI Panel")]
    public GameObject createRoomPanel;
    public InputField roomNameInput;
    public InputField maxPlayerAmountInput;

    [Header("Inside Room UI Panel")]
    public GameObject insideRoomPanel;
    public Text roomInfoText;
    public GameObject playerListPrefab;
    public GameObject playerListParent;
    public GameObject startGameButton;

    [Header("Room List UI Panel")]
    public GameObject roomListPanel;
    public GameObject roomListPrefab;
    public GameObject roomListParentGameObject;

    [Header("Join Random Room UI Panel")]
    public GameObject joinRandomRoomPanel;

    // Private 

    private Dictionary<string, RoomInfo> cachedRoomList;
    private Dictionary<string, GameObject> roomListGameObjects;
    private Dictionary<int, GameObject> playerListGameObjects;

    // ----------------------------------------------------------------------------- //

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(loginPanel.name);
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListGameObjects = new Dictionary<string, GameObject>();
        // Synchronize scenes for all clients
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Display connection status
        connectionStatusText.text = "Connection status : " + PhotonNetwork.NetworkClientState;
    }

    #endregion

    #region Private Methods

    private void UpdateRoomInfoText()
    {
        // Assign room name, player count and max palyer amount to Room Information field
        roomInfoText.text = "Room name: " + PhotonNetwork.CurrentRoom.Name + " " +
         PhotonNetwork.CurrentRoom.PlayerCount + " / " + PhotonNetwork.CurrentRoom.MaxPlayers;
    }

    private void ClearRoomList()
    {
        // Destroy every game object in room list
        foreach (var roomListObject in roomListGameObjects.Values)
        {
            Destroy(roomListObject);
        }
        roomListGameObjects.Clear();
    }

    // Join room button located inside RoomListPrefab
    private void OnJoinedRoomButtonClicked(string roomName)
    {
        // At this stage we do not need to stay in lobby
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        PhotonNetwork.JoinRoom(roomName);
    }

    #endregion

    #region Public Methods

    // Activate only relevant panel and deactivate the rest 
    public void ActivatePanel(string panelToActivate)
    {
        loginPanel.SetActive(panelToActivate.Equals(loginPanel.name));
        gameOptionsPanel.SetActive(panelToActivate.Equals(gameOptionsPanel.name));
        createRoomPanel.SetActive(panelToActivate.Equals(createRoomPanel.name));
        roomListPanel.SetActive(panelToActivate.Equals(roomListPanel.name));
        insideRoomPanel.SetActive(panelToActivate.Equals(insideRoomPanel.name));
        joinRandomRoomPanel.SetActive(panelToActivate.Equals(joinRandomRoomPanel.name));
    }

    #endregion

    #region UI Callbacks

    // Back button is located in the RoomListPanel
    public void OnBackButtonClicked()
    {
        // At this stage we do not need to stay in lobby
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivatePanel(gameOptionsPanel.name);
    }

    // Leave Game button is located in the InsideRoomPanel
    public void OnLeaveGameButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    // Start Game button is located in the InsideRoomPanel
    public void OnStartGameButtonClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("GameScene");
        }
    }

    // Cancel button is located in the CreateRoomPanel
    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameOptionsPanel.name);
    }

    // Create Room button is located in the CreateRoomPanel
    public void OnRoomCreateButtonClicked()
    {
        string roomName = roomNameInput.text;
        // If room name field is empty generate a random room name E.g. Room 324
        if (string.IsNullOrEmpty(roomName))
        {
            roomName = "Room " + Random.Range(1, 1000);
        }
        // Set room configurations
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(maxPlayerAmountInput.text);
        // Create room on a server
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    // Login button is located in the LoginPanel
    public void OnLoginButtonClicked()
    {
        string playerName = playerNameInput.text;
        if (!string.IsNullOrEmpty(playerName))
        {
            // Assign player name on server
            PhotonNetwork.LocalPlayer.NickName = playerName;
            // Connect to Photon server
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.Log("Player name is invalid!");
        }
    }

    // Random Room button is located in GameOptionsPanel
    public void OnJoinRandomRoomButtonClicked()
    {
        ActivatePanel(joinRandomRoomPanel.name);
        PhotonNetwork.JoinRandomRoom();
    }

    // Room List button is located in GameOptionsPanel
    public void OnShowRoomListButtonClicked()
    {
        // Must be in lobby to see the existing rooms list
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivatePanel(roomListPanel.name);
    }

    #endregion

    #region Photon Callbacks

    // On connection to Internet
    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    // On connection to Photon server
    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon server");
        ActivatePanel(gameOptionsPanel.name);
    }

    // Called for any update of the room-listing while in a lobby on the Master Server
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomList();

        foreach (RoomInfo room in roomList)
        {
            Debug.Log(room.Name);
            // Remove the lists that are closed, invisible or removed from the list
            // RemovedFromList is true when the room is hidden, closed or full
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList.Remove(room.Name);
                }
            }
            else
            {
                // Update cached room list
                if (cachedRoomList.ContainsKey(room.Name))
                {
                    cachedRoomList[room.Name] = room;
                }
                // Add the new room to the cached room list
                else
                {
                    cachedRoomList.Add(room.Name, room);
                }
            }
        }

        // Instantiate game object roomListPrefab
        foreach (RoomInfo room in cachedRoomList.Values)
        {
            GameObject roomListSingleGameObject = Instantiate(roomListPrefab);
            // Put newly instantiated room objects under Parent object for better arrangement
            roomListSingleGameObject.transform.SetParent(roomListParentGameObject.transform);
            // Avoid scaling issues
            roomListSingleGameObject.transform.localScale = Vector3.one;
            // Set name and player amount of a room 
            roomListSingleGameObject.transform.Find("RoomNameText").GetComponent<Text>().text = room.Name;
            roomListSingleGameObject.transform.Find("RoomPlayersText").GetComponent<Text>().text = room.PlayerCount +
            " / " + room.MaxPlayers;
            // Join relevant room 
            roomListSingleGameObject.transform.Find("JoinRoomButton").GetComponent<Button>().onClick.AddListener(()
            => OnJoinedRoomButtonClicked(room.Name));
            // Add room with its name and object to the room list
            roomListGameObjects.Add(room.Name, roomListSingleGameObject);
        }
    }

    // Get rid of redundant data on the background after leaving lobby
    public override void OnLeftLobby()
    {
        ClearRoomList();
        cachedRoomList.Clear();
    }

    // Called when this client created a room and entered it (OnJoinedRoom is being called as well)
    public override void OnCreatedRoom()
    {
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created");
    }

    // Called when any client joined an existing room 
    public override void OnJoinedRoom()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " has joined to " + PhotonNetwork.CurrentRoom.Name);
        ActivatePanel(insideRoomPanel.name);
        // Show "Start Game" button only for a player who created a room
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
        else
        {
            startGameButton.SetActive(false);
        }

        UpdateRoomInfoText();

        if (playerListGameObjects == null)
        {
            playerListGameObjects = new Dictionary<int, GameObject>();
        }

        // Instantiate player list game objects
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject playerListSingleGameObject = Instantiate(playerListPrefab);
            // Put newly instantiated player objects under Parent object for better arrangement
            playerListSingleGameObject.transform.SetParent(playerListParent.transform);
            // Avoid scaling issues
            playerListSingleGameObject.transform.localScale = Vector3.one;
            // Set player name field to relevant value
            playerListSingleGameObject.transform.Find("PlayerNameText").GetComponent<Text>().text = player.NickName;
            // Check if this player is "myself" by checking identified of player in current room
            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListSingleGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
            }
            else
            {
                playerListSingleGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
            }
            playerListGameObjects.Add(player.ActorNumber, playerListSingleGameObject);
        }
    }

    // Called when new remote player joins the same room we are in
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomInfoText();

        GameObject playerListSingleGameObject = Instantiate(playerListPrefab);
        // Put newly instantiated player objects under Parent object for better arrangement
        playerListSingleGameObject.transform.SetParent(playerListParent.transform);
        // Avoid scaling issues
        playerListSingleGameObject.transform.localScale = Vector3.one;
        // Set player name field to relevant value
        playerListSingleGameObject.transform.Find("PlayerNameText").GetComponent<Text>().text = newPlayer.NickName;
        // Check if this player is "myself" by checking identified of player in current room
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListSingleGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(true);
        }
        else
        {
            playerListSingleGameObject.transform.Find("PlayerIndicator").gameObject.SetActive(false);
        }
        playerListGameObjects.Add(newPlayer.ActorNumber, playerListSingleGameObject);
    }

    // Called when player leaves the room we are in
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfoText();

        // Destroy and remove player game object
        Destroy(playerListGameObjects[otherPlayer.ActorNumber].gameObject);
        playerListGameObjects.Remove(otherPlayer.ActorNumber);

        // If creator of a room left the game, then the next player becomes a leader of a room
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            startGameButton.SetActive(true);
        }
    }

    // In case we leave the room
    public override void OnLeftRoom()
    {
        ActivatePanel(gameOptionsPanel.name);
        // Destroy every player game object
        foreach (GameObject playerListSingleGameObject in playerListGameObjects.Values)
        {
            Destroy(playerListSingleGameObject);
        }
        // Clear player list 
        playerListGameObjects.Clear();
        playerListGameObjects = null;
    }

    // In case of failing to join random room 
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log(message);

        string roomName = "Room " + Random.Range(1, 1000);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 20;
        // Create random room 
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    #endregion
}
