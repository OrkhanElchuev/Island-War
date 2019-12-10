using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class Shooting : MonoBehaviourPunCallbacks
{
    public Camera FPS_Camera;
    public GameObject hitEffectPrefab;
    public GameObject deathCanvas;
    private float health = 100f;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        deathCanvas = GetComponent<GameObject>();
    }

    // Shooting via sending invisible Rays
    public void Attack()
    {
        RaycastHit hit;
        // Send a ray to the middle of the camera view(screen)
        Ray ray = FPS_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.Log(hit.collider.gameObject.name);
            // Display hit effect to all players in room
            photonView.RPC("CreateHitEffect", RpcTarget.All, hit.point);
            // Check if collided object is Player excluding ourselves
            if (hit.collider.gameObject.CompareTag("Player") && !hit.collider.gameObject.GetComponent<PhotonView>().IsMine)
            {
                hit.collider.gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 10f);
            }
        }
    }

    [PunRPC]
    public void TakeDamage(float damage, PhotonMessageInfo info)
    {
        health -= damage;
        Debug.Log(health);
        // If player is killed run dying animation
        if (health <= 0f)
        {
            Die();
            Debug.Log(info.Sender.NickName + " killed " + info.photonView.Owner.NickName);
        }
    }

    [PunRPC]
    public void CreateHitEffect(Vector3 position)
    {
        // Run hitting effect particle
        GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, position, Quaternion.identity);
        Destroy(hitEffectGameObject, 0.5f);
    }

    // Dying animation
    private void Die()
    {
        if (photonView.IsMine)
        {
            animator.SetBool("IsDead", true);
            StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        GameObject respawnText = GameObject.Find("RespawnText");
        // Set respawning period
        float respawnPeriod = 5.0f;
        while (respawnPeriod > 0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnPeriod -= 1.0f;
            // Deactivate player movement 
            transform.GetComponent<PlayerMovementController>().enabled = false;
            respawnText.GetComponent<Text>().text = "You are killed. Respawning at : " + respawnPeriod.ToString(".00");
            deathCanvas.SetActive(true);
        }
        // Reset player back to normal state
        animator.SetBool("IsDead", false);
        deathCanvas.SetActive(false);
        respawnText.GetComponent<Text>().text = " ";
        // Generate a random value for spawning point
        int randomPoint = Random.Range(-20, 20);
        transform.position = new Vector3(randomPoint, 0, randomPoint);
        transform.GetComponent<PlayerMovementController>().enabled = true;
        // Restore health points of newly spawned player for all clients
        photonView.RPC("RestoreHealth", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void RestoreHealth()
    {
        health = 100f;
    }
}
