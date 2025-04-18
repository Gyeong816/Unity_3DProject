using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon")] 
    public EmptyGun emptyGun;
    public Ak47 ak47;
    public M3 m3;
    

    public enum WeaponType 
    { 
        None, 
        AK47, 
        M3
    }
    
    
    public float IKWeight = 1f;
    public Transform LeftHandTarget;
    public Transform RightHandTarget;




    private float nextFireTime = 0;
    private WeaponType currentWeapon;
    
    
    
    private void Start()
    {
       emptyGun.gameObject.SetActive(true);
       ak47.gameObject.SetActive(false);
       m3.gameObject.SetActive(false);
       SetWeapon(WeaponType.None);
    }
    

    public void SetWeapon(WeaponType weaponType)
    {
        emptyGun.gameObject.SetActive(false);
        ak47.gameObject.SetActive(false);
        m3.gameObject.SetActive(false);
        
        switch (weaponType)
        {
            case WeaponType.None:
              
                emptyGun.gameObject.SetActive(true);
                LeftHandTarget = emptyGun.LeftHandTarget;
                RightHandTarget = emptyGun.RightHandTarget;
                break;
            
          case WeaponType.AK47:
              
              ak47.gameObject.SetActive(true);
              LeftHandTarget = ak47.LeftHandTarget;
              RightHandTarget = ak47.RightHandTarget;
              break;
          
          case WeaponType.M3:
              m3.gameObject.SetActive(true);
              LeftHandTarget = m3.LeftHandTarget;
              RightHandTarget = m3.RightHandTarget;
              break;
            
            default:
                break;
        }
        
        currentWeapon = weaponType;
    }

    public Transform GetCurrentWeaponTransform()
    {
        switch (currentWeapon)
        {
            case WeaponType.None:
                return emptyGun.transform;
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
            case WeaponType.None:
                emptyGun.Fire();
                break;
            case WeaponType.AK47:
                if (Time.time >= nextFireTime)
                {
                    nextFireTime = Time.time + ak47.data.fireRate;
                    ak47.Fire();
                }
                break;
            case WeaponType.M3:
                m3.Fire();
                break;
            default:
                break;
        }
    }
  

}
