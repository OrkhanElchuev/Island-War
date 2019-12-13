using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FPSGameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            if (playerPrefab != null)
            {
                // Generate random value for 3 spots
                int randomSpawningAreas = Random.Range(1, 4);
                switch (randomSpawningAreas)
                {
                    case 1:
                        // Generate a random point for spot 1 to spawn the player
                        int randomPointX_1 = Random.Range(25, 40);
                        int randomPointZ_1 = Random.Range(38, 48);
                        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPointX_1, 0f, randomPointZ_1), Quaternion.identity);
                        break;
                    case 2:
                        // Generate a random point for spot 2 to spawn the player
                        int randomPointX_2 = Random.Range(-36, -20);
                        int randomPointZ_2 = Random.Range(-15, -25);
                        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPointX_2, 0f, randomPointZ_2), Quaternion.identity);
                        break;
                    case 3:
                        // Generate a random point for spot 3 to spawn the player
                        int randomPointX_3 = Random.Range(31, 45);
                        int randomPointZ_3 = Random.Range(-8, 5);
                        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(randomPointX_3, 0f, randomPointZ_3), Quaternion.identity);
                        break;
                }
            }
            else
            {
                Debug.Log("Player prefab is not set");
            }
        }
    }
}
