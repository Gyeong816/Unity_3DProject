using System.Collections;
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
    public float aimDistance = 2f;

    [Header("Camera Offsets")]
    public Vector3 cameraOffset = new Vector3(0.5f, 1.6f, 0f);
    public Vector3 crouchCameraOffset = new Vector3(0.5f, 1.3f, 0f);

    [Header("Wall Collision Settings")]
    public LayerMask obstacleMask;
    public float sphereRadius = 0.3f;
    public float cameraSmoothSpeed = 12f;

    [Header("Shake Settings")]
    public float shakeStrength = 0.05f;
    public float shakeDuration = 0.1f;

    private float yaw;
    private float pitch;

    private float shakeTimer = 0f;
    private Vector3 shakeOffset = Vector3.zero;

    private bool isAiming;
    private bool isCrouching;
    private bool isCamOff;

    private Vector3 smoothedShoulderPos;
    private Quaternion smoothedRotation;

    void Start()
    {
        smoothedShoulderPos = transform.position + transform.forward;
        smoothedRotation = transform.rotation;
    }

    void LateUpdate()
    {
        if (!isCamOff)
        {
            CameraMove();
        }
    }

    private void CameraMove()
    {
        // 입력 처리
        yaw += Input.GetAxis("Mouse X") * mouseSensitivityX;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion targetRotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 baseOffset = isCrouching ? crouchCameraOffset : cameraOffset;
        Vector3 targetShoulderPos = target.position + targetRotation * baseOffset;

        // 조준 상태일 때 보간 없이 적용
        if (isAiming)
        {
            smoothedShoulderPos = targetShoulderPos;
            smoothedRotation = targetRotation;
        }
        else
        {
            smoothedShoulderPos = Vector3.Lerp(smoothedShoulderPos, targetShoulderPos, Time.deltaTime * cameraSmoothSpeed);
            smoothedRotation = Quaternion.Slerp(smoothedRotation, targetRotation, Time.deltaTime * cameraSmoothSpeed);
        }

        float targetCamDistance = isAiming ? aimDistance : distance;
        Vector3 camDirection = smoothedRotation * Vector3.back;
        Vector3 desiredCameraPos = smoothedShoulderPos + camDirection * targetCamDistance;
        Vector3 finalCameraPos = desiredCameraPos;

        // 벽 충돌 보정
        Vector3 rayStart = target.position + Vector3.up * 1.5f;
        Vector3 rayDir = (desiredCameraPos - rayStart).normalized;
        float rayDist = Vector3.Distance(rayStart, desiredCameraPos);

        RaycastHit[] hits = Physics.SphereCastAll(rayStart, sphereRadius, rayDir, rayDist, obstacleMask, QueryTriggerInteraction.Ignore);
        if (hits.Length > 0)
        {
            float closestDist = rayDist;
            foreach (var hit in hits)
            {
                if (hit.distance < closestDist)
                {
                    closestDist = hit.distance;
                }
            }
            finalCameraPos = rayStart + rayDir * closestDist;
        }

        // 흔들림 처리
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            float intensity = Mathf.Lerp(0f, shakeStrength, shakeTimer / shakeDuration);
            shakeOffset = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ) * intensity;
        }
        else
        {
            shakeOffset = Vector3.zero;
        }

        // 카메라 위치 및 회전 처리
        if (isAiming)
        {
            transform.position = finalCameraPos + shakeOffset;
            transform.rotation = Quaternion.LookRotation(smoothedShoulderPos - transform.position);
        }
        else
        {
            Vector3 smoothPos = Vector3.Lerp(transform.position, finalCameraPos + shakeOffset, Time.deltaTime * cameraSmoothSpeed);
            transform.position = smoothPos;

            Quaternion lookRot = Quaternion.LookRotation(smoothedShoulderPos - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * cameraSmoothSpeed);
        }
    }


    public void SetShake()
    {
        shakeTimer = shakeDuration;
    }

    public void SetAiming(bool aiming)
    {
        isAiming = aiming;
    }

    public void SetCrouch(bool crouching)
    {
        isCrouching = crouching;
    }

    public void SetCamOn(bool camOff)
    {
        isCamOff = camOff;
    }
}
