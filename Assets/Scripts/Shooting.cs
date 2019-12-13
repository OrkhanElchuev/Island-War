using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera FPS_Camera;
    public GameObject hitEffectPrefab;
    public GameObject bloodEffect;

    private Text healthText;
    private float health = 100f;
    private Animator playerAnimator;
    private Animator hurtPanelAnimator;
    private GameObject deathPanel;
    private GameObject respawnText;

    private void Start()
    {
        InitialSetup();
    }

    // Shooting via sending invisible Rays
    public void Attack()
    {
        RaycastHit hit;
        // Send a ray to the middle of the camera view(screen)
        Ray ray = FPS_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));
        float randomDamage = Random.Range(5f, 20f);
        if (Physics.Raycast(ray, out hit, 100))
        {
            // Debug.Log(hit.collider.gameObject.name); // <<< Only for Debugging purposes

            // Display hit effect to all players in room
            photonView.RPC("CreateHitEffect", RpcTarget.All, hit.point);
            // Check if collided object is Player excluding ourselves
            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, randomDamage);
                photonView.RPC("CreateBloodEffect", RpcTarget.All, hit.point);
            }
        }
    }

    #region PunRPC Methods

    [PunRPC]
    public void TakeDamage(float damage, PhotonMessageInfo info)
    {
        health -= damage;
        UpdateHealthText();
        HandleHurtPanelAnimation();
        // If player is killed run dying animation
        if (health <= 0f)
        {
            // Avoid having negative health
            health = 0f;
            Die();
            // Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName); // <<< Only for Debugging purposes
        }
    }

    [PunRPC]
    public void CreateHitEffect(Vector3 position)
    {
        // Run hitting effect particle
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.5f);
    }

    [PunRPC]
    public void CreateBloodEffect(Vector3 position)
    {
        // Run blood effect
        GameObject hitEffectGameObject = Instantiate(bloodEffect, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.7f);
    }

    [PunRPC]
    public void RestoreHealth()
    {
        health = 100f;
    }

    #endregion

    #region Private Methods

    private void InitialSetup()
    {
        healthText = GameObject.Find("HealthPoints").GetComponent<Text>();
        deathPanel = GameObject.Find("DeathPanel");
        if (photonView.IsMine)
        {
            deathPanel.SetActive(false);
        }
        playerAnimator = GetComponent<Animator>();
        hurtPanelAnimator = GetComponent<Animator>();
    }

    // Display red panel for a short period of time each time damage is received
    private void HandleHurtPanelAnimation()
    {
        hurtPanelAnimator.SetTrigger("IsHurt");
    }

    private void UpdateHealthText()
    {
        healthText.GetComponent<Text>().text = health.ToString();
    }

    // Dying animation
    private void Die()
    {
        if (photonView.IsMine)
        {
            playerAnimator.SetBool("IsDead", true);
            StartCoroutine(Respawn());
        }
    }

    // Resets player UI and animation state to initial values
    private void ResetPlayer()
    {
        playerAnimator.SetBool("IsDead", false);
        respawnText.GetComponent<Text>().text = " ";
        deathPanel.SetActive(false);
        UpdateHealthText();
    }

    #endregion

    IEnumerator Respawn()
    {
        respawnText = GameObject.Find("RespawnText");
        // Set respawning period
        float respawnPeriod = 5.0f;
        // Count down 
        while (respawnPeriod > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnPeriod -= 1.0f;
            deathPanel.SetActive(true);
            respawnText.GetComponent<Text>().text = "You are killed. Respawning at : " + respawnPeriod.ToString(".00");
        }
        ResetPlayer();
        // Generate a random value for spawning point
        int randomPoint = Random.Range(-20, 20);
        transform.position = new Vector3(randomPoint, 0, randomPoint);
        // Restore health points of newly spawned player for all clients
        photonView.RPC("RestoreHealth", RpcTarget.AllBuffered);
        UpdateHealthText();
    }
}
