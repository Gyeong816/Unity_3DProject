using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("")]
    public Transform target;

    [Header("")]
    public float mouseSensitivityX = 3f;
    public float mouseSensitivityY = 1.5f;
    public float minY = -30f;
    public float maxY = 60f;

    [Header("")]
    public float distance = 5f;
    public float aimdistance = 2f;
    public Vector3 cameraOffset = new Vector3(0.5f, 1.6f, 0f); // �� ������/���� ������ ����

    private float yaw;
    private float pitch;
    
    
    private bool isAiming;

    public void SetAiming(bool aiming)
    {
        isAiming = aiming;
    }

    void LateUpdate()
    {
        if (target == null) return;

        yaw += Input.GetAxis("Mouse X") * mouseSensitivityX;
        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivityY;
        pitch = Mathf.Clamp(pitch, minY, maxY);

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 offsetDir = isAiming ? rotation * Vector3.back * aimdistance : rotation * Vector3.back * distance;

        // �������� ������/���� �̵���Ų ��ġ
        Vector3 targetPosition = target.position + rotation * cameraOffset;

        transform.position = targetPosition + offsetDir;
        transform.LookAt(targetPosition);
    }
    
}
