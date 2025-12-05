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

  void Start()
  {
    InitialSetup();
    HandleRelevantView();
  }

  private void Update()
  {
    // To avoid "multiple audio listener" warning
    if (!photonView.IsMine)
    {
      FindObjectOfType<Camera>().GetComponent<AudioListener>().enabled = false;
    }
  }

  private void InitialSetup()
  {
    shooter = GetComponent<Shooting>();
    playerMovementController = GetComponent<PlayerMovementController>();
    animator = GetComponent<Animator>();
  }

  private void HandleRelevantView()
  {
    if (photonView.IsMine)
    {
      foreach (GameObject go in FPS_HandsChildGameObjects)
      {
        go.SetActive(true);

        // make this weapon/hand object follow the camera
        var follow = go.AddComponent<GunFollowCamera>();
        follow.Init(FPSCamera);
      }

      foreach (GameObject go in SoldierChildGameObjects)
        go.SetActive(false);

      GameObject playerUIGameObject = Instantiate(playerUIPrefab);

      playerMovementController.joystick =
          playerUIGameObject.transform.Find("Fixed Joystick").GetComponent<Joystick>();

      playerMovementController.fixedTouchField =
          playerUIGameObject.transform.Find("RotationTouchPanel").GetComponent<FixedTouchField>();

      playerUIGameObject.transform.Find("Shoot")
          .GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => shooter.Attack());

      FPSCamera.enabled = true;
      animator.SetBool("IsPlayerModel", false);
    }
    else
    {
      foreach (GameObject go in FPS_HandsChildGameObjects)
        go.SetActive(false);

      foreach (GameObject go in SoldierChildGameObjects)
        go.SetActive(true);

      playerMovementController.enabled = false;
      GetComponent<UnityStandardAssets.Characters.FirstPerson.
                  RigidbodyFirstPersonController>().enabled = false;
      FPSCamera.enabled = false;

      animator.SetBool("IsPlayerModel", true);
    }
  }
}
