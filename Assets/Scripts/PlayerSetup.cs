using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.UI;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] FPS_HandsChildGameObjects;
    public GameObject[] SoldierChildGameObjects;
    public GameObject playerUIPrefab;
    public Camera FPSCamera;

    private PlayerMovementController playerMovementController;
    private Animator animator;
    private Shooting shooter;

    // Start is called before the first frame update
    void Start()
    {
        InitialSetup();
        HandleRelevantView();
    }

    private void Update()
    {

    }
    
    private void InitialSetup()
    {
        // To avoid multiple audio listener in one scene warning
        if (!photonView.IsMine)
        {
            FindObjectOfType<Camera>().GetComponent<AudioListener>().enabled = false;
        }
        shooter = GetComponent<Shooting>();
        playerMovementController = GetComponent<PlayerMovementController>();
        animator = GetComponent<Animator>();
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
            playerUIGameObject.transform.Find("Shoot").GetComponent<Button>().onClick.AddListener(() => shooter.Attack());

            FPSCamera.enabled = true;
            // Set hand model animation
            animator.SetBool("IsPlayerModel", false);
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
            // Set Player model animation
            animator.SetBool("IsPlayerModel", true);
        }
    }
}
