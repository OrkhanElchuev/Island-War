using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FPSGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    #region Unity Methods

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (playerPrefab != null)
            {
                // Generate a random point for spawning player
                int randomPoint = Random.Range(-10, 10);
                PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPoint, 0f, randomPoint), Quaternion.identity);
            }
            else
            {
                Debug.Log("Player prefab is not set");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    #endregion
}
