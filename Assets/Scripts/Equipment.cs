using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Equipment : MonoBehaviour
{
    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    public Transform WeaponTransform;
   [Range(0f, 1f)] public float IKWeight = 1f;
   
   private Quaternion originalLocalRotation;
   private Vector3 originalLocalPosition;
       
   
   public void Fire()
   {
       originalLocalRotation = transform.localRotation;
       originalLocalPosition = transform.localPosition;
       Recoil();
   }
   
  
   
   private void Recoil()
   {
       // 트윈 충돌 방지
       transform.DOKill();

       // 회전 + 위치 반동을 동시에 시작하고, 동시에 복구하는 시퀀스
       Sequence recoilSequence = DOTween.Sequence();

       // 반동 회전 및 위치 (0.05초 동안 동시에 실행)
       recoilSequence.Append(
           transform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(-10f, 0f, 0f), 0.05f)
       );
       recoilSequence.Join(
           transform.DOLocalMove(originalLocalPosition + new Vector3(0f, 0f, -0.05f), 0.05f)
       );

       // 복구 회전 및 위치 (0.05초 동안 동시에 실행)
       recoilSequence.Append(
           transform.DOLocalRotateQuaternion(originalLocalRotation, 0.05f)
       );
       recoilSequence.Join(
           transform.DOLocalMove(originalLocalPosition, 0.05f)
       );
   }
}
