using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTargetUpdater : MonoBehaviour
{
    public Camera mainCamera;
    public Transform aimTarget;
    public float distance = 30f;

    void Update()
    {
       
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); 
        
        aimTarget.position = ray.origin + ray.direction * distance;
        
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.grey);
    }
}
