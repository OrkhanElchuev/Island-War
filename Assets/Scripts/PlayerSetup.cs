using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] FPS_HandsChildGameObjects;
    public GameObject[] SoldierChildGameObjects;
    public GameObject playerUIPrefab;
    public Camera FPSCamera;

    private PlayerMovementController playerMovementController;

    // Start is called before the first frame update
    void Start()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
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

            // Instantiate Player UI only for myself
            GameObject playerUIGameObject = Instantiate(playerUIPrefab);
            // Assign Joystick and player rotation field
            playerMovementController.joystick = 
                playerUIGameObject.transform.Find("Fixed Joystick").GetComponent<Joystick>();
            playerMovementController.fixedTouchField =
                playerUIGameObject.transform.Find("RotationTouchPanel").GetComponent<FixedTouchField>();
            FPSCamera.enabled = true;    
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
           
            // Disable joystick components and camera 
            playerMovementController.enabled = false;
            GetComponent<RigidbodyFirstPersonController>().enabled = false;
            FPSCamera.enabled = false;
        }
    }
}
