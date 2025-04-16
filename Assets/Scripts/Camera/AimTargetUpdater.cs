using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTargetUpdater : MonoBehaviour
{
    public Camera mainCamera;
    public Transform aimTarget;
    public float maxDistance = 100f;
    public LayerMask aimLayerMask; 

    void Update()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, aimLayerMask))
        {
            aimTarget.position = hit.point;
      
        }
        else
        {
            aimTarget.position = ray.origin + ray.direction * maxDistance;
        }

        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.yellow);
    }
}
