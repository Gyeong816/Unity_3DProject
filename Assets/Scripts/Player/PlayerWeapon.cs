using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon")] 
    public Ak47 ak47;
    public M3 m3;

    
    
    [Range(0f, 1f)] public float IKWeight = 1f;
    public Transform LeftHandTarget;
    public Transform RightHandTarget;
    
    
    
    private bool isFiring;
    
    public enum WeaponType 
    { 
        None, 
        AK47, 
        M3
    }
    
    private WeaponType currentWeapon;
    
    
    
    
    private void Start()
    {
      ak47.gameObject.SetActive(false);
      m3.gameObject.SetActive(false);
    }



    public void SetWeapon(WeaponType weaponType)
    {
        ak47.gameObject.SetActive(false);
        m3.gameObject.SetActive(false);
        
        switch (weaponType)
        {
          case WeaponType.AK47:
              
              ak47.gameObject.SetActive(true);
              LeftHandTarget = ak47.LeftHandTarget;
              RightHandTarget = ak47.RightHandTarget;
              
              break;
          case WeaponType.M3:
              m3.gameObject.SetActive(true);
              break;
          
        }
        
        currentWeapon = weaponType;
    }

    public Transform GetCurrentWeaponTransform()
    {
        switch (currentWeapon)
        {
            case WeaponType.AK47:
                return ak47.transform;
            case WeaponType.M3:
                return m3.transform;
            default:
                return null;
        }
    }
    
    public void Fire()
    {
        switch (currentWeapon)
        {
            case WeaponType.AK47:
                ak47.Fire();
                break;
            case WeaponType.M3:
                
                break;
        }
    }
  
    public void SetFire(bool isfiring)
    {
        isFiring = isfiring;
    }
}
