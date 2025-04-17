using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTargetUpdater : MonoBehaviour
{
    public Camera mainCamera;
    public Transform gunAimTarget;
    public float maxDistance = 100f;
    public LayerMask aimLayerMask; 

    public Transform bodyAimTarget;
    public float distance = 30f;
    
    void Update()
    {
        Ray gunRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(gunRay, out RaycastHit hit, maxDistance, aimLayerMask))
        {
            gunAimTarget.position = hit.point;
      
        }
        else
        {
            gunAimTarget.position = gunRay.origin + gunRay.direction * maxDistance;
        }

        Debug.DrawRay(gunRay.origin, gunRay.direction * maxDistance, Color.yellow);
        
        
        
        
        Ray bodyrRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); 
        
        bodyAimTarget.position = bodyrRay.origin + bodyrRay.direction * distance;
        
        Debug.DrawRay(bodyrRay.origin, bodyrRay.direction * distance, Color.grey);
        
    }
    
 
   


}
