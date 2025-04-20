using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Ak47 : MonoBehaviour
{
    public GunData data;

    public GameObject bulletImpactEffect;
    public GameObject enemyImpactEffect;   
    public GameObject groundImpactEffect;  

    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    
    public Transform WeaponRoot;
    public Transform Muzzle;
   
    
    [Range(0f, 1f)] public float IKWeight = 1f;
    
    
    
    
    
    private Quaternion originalLocalRotation;
    private bool isFiring;
   
   
   
    

   private void Update()
   {
       
       Debug.DrawRay(Muzzle.transform.position, Muzzle.transform.forward * data.range, Color.red);
   }


   public void Fire()
   {
       originalLocalRotation = transform.localRotation;
       Recoil();

      

        Ray ray = new Ray(Muzzle.transform.position, Muzzle.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, data.range))
        {
            CombatSystem.Instance.ApplyDamage(hit, data.damage);

            
            Quaternion rotation = Quaternion.LookRotation(hit.normal);

            
            int hitLayer = hit.collider.gameObject.layer;

            switch (hitLayer)
            {
                case int n when n == LayerMask.NameToLayer("Enemy"):
                    Instantiate(enemyImpactEffect, hit.point, rotation);
                    break;
                case int n when n == LayerMask.NameToLayer("Ground"):
                    Instantiate(groundImpactEffect, hit.point, rotation);
                    break;

                default:
                    Instantiate(bulletImpactEffect, hit.point, rotation); 
                    break;
            }
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
