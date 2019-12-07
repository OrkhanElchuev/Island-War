using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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
        // Activating game options panel after successfull connection to the Photon server
        ActivatePanel(gameOptionsPanel.name);
    }

    #endregion
}
