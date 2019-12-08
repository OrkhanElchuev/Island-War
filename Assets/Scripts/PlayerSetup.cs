using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] FPS_HandsChildGameObjects;
    public GameObject[] SoldierChildGameObjects;

    // Start is called before the first frame update
    void Start()
    {
        HandleRelevantView();
    }

    private void HandleRelevantView()
    {
        // For us show only hand and gun (deactivate body model)
        if (photonView.IsMine)
        {
            foreach (GameObject gameObject in FPS_HandsChildGameObjects)
            {
                gameObject.SetActive(true);
            }

            foreach (GameObject gameObject in SoldierChildGameObjects)
            {
                gameObject.SetActive(false);
            }
        }
        // For the rest of the players show our body model 
        else
        {
            foreach (GameObject gameObject in FPS_HandsChildGameObjects)
            {
                gameObject.SetActive(false);
            }

            foreach (GameObject gameObject in SoldierChildGameObjects)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
