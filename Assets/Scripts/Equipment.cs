using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Equipment : MonoBehaviour
{
    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    
    public Transform WeaponRoot;
    
    
   [Range(0f, 1f)] public float IKWeight = 1f;
   
   private Quaternion originalLocalRotation;
       
   
   public void Fire()
   {
       originalLocalRotation = transform.localRotation;
       Recoil();
   }
   
  
   
   private void Recoil()
   {
       

       Sequence recoilSequence = DOTween.Sequence();

       float pitch = Random.Range(-2f, -1f);   
       float yaw = Random.Range(-3f, 3f);    
       float roll = Random.Range(-2f, 2f);
       
     
       recoilSequence.Append(
           transform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(pitch, yaw, roll), 0.05f)
       );
       recoilSequence.Join(
           WeaponRoot.transform.DOLocalMove( new Vector3(0f, 0.004f, 0.310f), 0.05f)
       );
       

     
       recoilSequence.Append(
           transform.DOLocalRotateQuaternion(originalLocalRotation, 0.05f)
       );
       recoilSequence.Join(
           WeaponRoot.transform.DOLocalMove(new Vector3(0f, 0.004f, 0.358f), 0.05f)
       );
   }
}
