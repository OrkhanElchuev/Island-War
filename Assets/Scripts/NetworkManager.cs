using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
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

    [Header("Room List UI Panel")]
    public GameObject roomListPanel;

    [Header("Join Random Room UI Panel")]
    public GameObject joinRandomRoomPanel;

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        ActivatePanel(loginPanel.name);
    }

    // Update is called once per frame
    void Update()
    {
        // Display connection status
        connectionStatusText.text = "Connection status : " + PhotonNetwork.NetworkClientState;
    }

    #endregion

    #region Private Methods


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

    // Cancel button which is located on CreateRoomPanel
    public void OnCancelButtonClicked()
    {
        ActivatePanel(gameOptionsPanel.name);
    }

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
    }

    #endregion
}
