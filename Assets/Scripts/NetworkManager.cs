using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Login UI")]
    public InputField playerNameInput;
    
    [Header("Connection Status")]
    public Text connectionStatusText;


    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Display connection status
        connectionStatusText.text = "Connection status : " + PhotonNetwork.NetworkClientState;
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
    }

    #endregion
}
