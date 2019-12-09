﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    public Camera FPS_Camera;
    public GameObject hitEffectPrefab;

    // Shooting via sending invisible Rays
    public void Attack()
    {
        RaycastHit hit;
        // Send a ray to the middle of the camera view(screen)
        Ray ray = FPS_Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f));

        if (Physics.Raycast(ray, out hit, 100))
        {
            Debug.Log(hit.collider.gameObject.name);
            // Run hitting effect particle
            GameObject hitEffectGameObject = Instantiate(hitEffectPrefab, hit.point, Quaternion.identity);
            Destroy(hitEffectGameObject, 0.5f);
        }
    }
}
