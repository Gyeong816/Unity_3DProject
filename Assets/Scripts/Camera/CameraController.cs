using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public Transform target;

    [Header("Sensitivity")]
    public float mouseSensitivityX = 3f;
    public float mouseSensitivityY = 1.5f;
    public float minY = -30f;
    public float maxY = 60f;

    [Header("Camera Distances")]
    public float distance = 5f;
    public float aimdistance = 2f;

    [Header("Camera Offsets")]
    public Vector3 cameraOffset = new Vector3(0.5f, 1.6f, 0f);
    public Vector3 crouchCameraOffset = new Vector3(0.5f, 1.3f, 0f);

    [Header("Shake Settings")]
    private Vector3 shakeOffset = Vector3.zero;
    public float shakeStrength = 0.05f;

    private float yaw;
    private float pitch;

    private bool isAiming;
    private bool isCrouching;
    private bool isFiring;
    private bool isCamOff;

    void LateUpdate()
    {
        if (!isCamOff)
        {
            CameraMove();
        }
    }

    private void CameraMove()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivityX;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 baseOffset = isCrouching ? crouchCameraOffset : cameraOffset;
        Vector3 desiredCameraPos = target.position + rotation * baseOffset;
        Vector3 cameraDir = rotation * Vector3.back;

        float defaultDistance = isAiming ? aimdistance : distance;
        float camDistance = defaultDistance;

        // 벽 충돌 감지 (뒤 방향)
        if (Physics.Raycast(desiredCameraPos, cameraDir, out RaycastHit hit, camDistance))
        {
            camDistance = hit.distance - 0.4f;
        }

        // 최종 카메라 위치 계산
        Vector3 cameraPos = desiredCameraPos + cameraDir * camDistance;

        // 카메라 주변 방향으로 레이 쏘기 (방향별 감지 거리 다르게)
        Vector3[] checkDirs =
        {
            rotation * Vector3.right,                      // 오른쪽
            rotation * -Vector3.right,                     // 왼쪽
            rotation * Vector3.up,                         // 위
            rotation * -Vector3.up,                        // 아래
            rotation * -Vector3.forward,                   // 뒤
            rotation * Vector3.forward,                    // 정면
            rotation * Quaternion.Euler(0, -40f, 0) * Vector3.forward,  // 시야 왼쪽
            rotation * Quaternion.Euler(0, 40f, 0) * Vector3.forward             // 시야 오른쪽
        };

        float[] checkLengths =
        {
            0.6f,  // right
            0.6f,  // left
            0.3f,  // up
            0.3f,  // down
            0.3f,  // back
            0.7f,  // view left
            0.7f   // view right
        };

     

        // 총기 흔들림
        if (isFiring)
        {
            shakeOffset = new Vector3(
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f),
                UnityEngine.Random.Range(-1f, 1f)
            ) * shakeStrength;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        transform.position = cameraPos + shakeOffset;
        transform.LookAt(desiredCameraPos);
    }

    public void SetShake(bool isfiring)
    {
        isFiring = isfiring;
    }

    public void SetAiming(bool aiming)
    {
        isAiming = aiming;
    }

    public void SetCrouch(bool crouching)
    {
        isCrouching = crouching;
    }

    public void SetCamOn(bool iscamOff)
    {
        isCamOff = iscamOff;
    }
}