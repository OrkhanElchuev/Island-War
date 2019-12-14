using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PauseMenu : MonoBehaviour
{
    private GameObject pausePanel;

    // Start is called before the first frame update
    void Start()
    {
        pausePanel = GameObject.Find("PausePanel");
        // Find Pause panel and initially hide it
        pausePanel.SetActive(false);
    }

    public void ActivatePausePanel()
    {
        pausePanel.SetActive(true);
    }

    // Resume button is located in Pause Panel (inside PlayerUI canvas)
    public void OnResumeButtonClick()
    {
        pausePanel.SetActive(false);
    }

    public void OnHomeButtonClick()
    {
        pausePanel.SetActive(false);
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel("LobbyScene");
        PhotonNetwork.ReconnectAndRejoin();
    }
}
