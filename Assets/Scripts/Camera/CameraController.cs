using System;
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
    public Vector3 cameraOffset = new Vector3(0.5f, 1.6f, 0f); 
    public Vector3 crouchCameraOffset = new Vector3(0.5f, 1.3f, 0f); 

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
        Vector3 offsetDir = isAiming ? rotation * Vector3.back * aimdistance : rotation * Vector3.back * distance;
        Vector3 baseOffset = isCrouching ? crouchCameraOffset : cameraOffset;
        
        
        
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
        
        Vector3 targetPosition = target.position + rotation * baseOffset + shakeOffset;

        transform.position = targetPosition + offsetDir;
        transform.LookAt(targetPosition);
        
        
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
