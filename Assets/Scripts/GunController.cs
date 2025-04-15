using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class GunController : MonoBehaviour
{
   
    [Header("Recoil Target")]
    public Transform recoilTransform;  // 반동 대상 (총 루트 또는 총기 그립 포인트)

    [Header("Recoil Settings")]
    public float recoilStrength = 1f;
    public float recoilRecoverySpeed = 8f;
    public float recoilPositionStrength = 0.1f;

    private Quaternion initialRotation;
    private Vector3 initialPosition;

    private Vector3 recoilRotationOffset;
    private Vector3 recoilPositionOffset;
    private float recoilTime = 0f;

    void Start()
    {
        initialRotation = recoilTransform.localRotation;
        initialPosition = recoilTransform.localPosition;
    }

    public void Fire()
    {
        recoilTime = 1f;

        // 회전 반동 (Quaternion.Euler에 사용)
        recoilRotationOffset = new Vector3(
            Random.Range(-10f, 10f),   // 좌우 흔들림 (x)
            Random.Range(-6f, 6f),     // 상하 흔들림 (y)
            -0.4f                       // 뒤로 기울이기 (z)
        );

        // 위치 반동 (뒤로 실제로 밀림)
        recoilPositionOffset = new Vector3(
            -10f* recoilPositionStrength,
            0f,
            0f
        );
    }

    void Update()
    {
        if (recoilTime > 0)
        {
            // 회전 적용
            Quaternion recoilRot = initialRotation * Quaternion.Euler(recoilRotationOffset * recoilTime);
            recoilTransform.localRotation = Quaternion.Slerp(recoilTransform.localRotation, recoilRot, Time.deltaTime * recoilRecoverySpeed);

            // 위치 적용
            Vector3 recoilPos = initialPosition + recoilPositionOffset * recoilTime;
            recoilTransform.localPosition = Vector3.Slerp(recoilTransform.localPosition, recoilPos, Time.deltaTime * recoilRecoverySpeed);

            // 시간 감소
            recoilTime -= Time.deltaTime * recoilRecoverySpeed;
        }
        else
        {
            // 원래 상태로 복귀
            recoilTransform.localRotation = Quaternion.Slerp(recoilTransform.localRotation, initialRotation, Time.deltaTime * recoilRecoverySpeed);
            recoilTransform.localPosition = Vector3.Slerp(recoilTransform.localPosition, initialPosition, Time.deltaTime * recoilRecoverySpeed);
        }
    }
}
