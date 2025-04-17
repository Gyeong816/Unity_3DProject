using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Gun : MonoBehaviour
{
    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    
    public Transform WeaponRoot;
    public Transform Muzzle;
    public float WeaponDamage;
    
    
    public float fireRate = 0.1f; 
    private float nextFireTime = 0f;
    
    public float rayLength = 100f;
    
   [Range(0f, 1f)] public float IKWeight = 1f;
   
   private Quaternion originalLocalRotation;
       
   private bool isFiring;
   
   public void SetFire(bool isfiring)
   {
       isFiring = isfiring;
   }

   private void Update()
   {
       if (isFiring && Time.time >= nextFireTime)
       {
           nextFireTime = Time.time + fireRate;
           Fire();
       }

       Debug.DrawRay(Muzzle.transform.position, Muzzle.transform.forward * rayLength, Color.red);
   }


   private void Fire()
   {
       originalLocalRotation = transform.localRotation;
       Recoil();

       Ray ray = new Ray(Muzzle.transform.position, Muzzle.transform.forward);
    
       if (Physics.Raycast(ray, out RaycastHit hit, rayLength))
       {
           CombatSystem.Instance.ApplyDamage(hit,WeaponDamage); 
       }
   }
   private void Recoil()
   {
       

       Sequence recoilSequence = DOTween.Sequence();

       float pitch = Random.Range(-2f, -1f);   
       float yaw = Random.Range(-3f, 3f);    
       float roll = Random.Range(-2f, 2f);
       
     
       recoilSequence.Append(
           transform.DOLocalRotateQuaternion(originalLocalRotation * Quaternion.Euler(pitch, yaw, roll), 0.1f)
       );
       recoilSequence.Join(
           WeaponRoot.transform.DOLocalMove( new Vector3(0f, 0.004f, 0.310f), 0.1f)
       );
       

     
       recoilSequence.Append(
           transform.DOLocalRotateQuaternion(originalLocalRotation, 0.1f)
       );
       recoilSequence.Join(
           WeaponRoot.transform.DOLocalMove(new Vector3(0f, 0.004f, 0.358f), 0.1f)
       );
   }
}
