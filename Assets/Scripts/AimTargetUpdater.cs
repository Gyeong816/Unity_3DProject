using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimTargetUpdater : MonoBehaviour
{
    public Camera mainCamera;
    public Transform aimTarget;
    public float distance = 30f; // 조준 거리

    void Update()
    {
        Vector3 center = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = mainCamera.ScreenPointToRay(center);

        aimTarget.position = ray.origin + ray.direction * distance;

        // 디버그용 레이 표시 (씬 뷰에서만 보임)
        Debug.DrawRay(ray.origin, ray.direction * distance, Color.red);
    }
}
