using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerEquipment : MonoBehaviour
{
    [Header("PlayerEquipment")] 
    public EmptyGun emptyGun;
    public Ak47 ak47;
    public M3 m3;
    

    public enum EquipmentType 
    { 
        None, 
        Backpack, 
        Helmet,
        Vest
    }
    
    
 
    
    
    
    
    private EquipmentType currentEquipment;
    
    
    
    private void Start()
    {
       emptyGun.gameObject.SetActive(true);
       ak47.gameObject.SetActive(false);
       m3.gameObject.SetActive(false);
       SetWeapon(EquipmentType.None);
    }
    

    public void SetWeapon(EquipmentType equipmentType)
    {
        emptyGun.gameObject.SetActive(false);
        ak47.gameObject.SetActive(false);
        m3.gameObject.SetActive(false);
        
        switch (equipmentType)
        {
            case EquipmentType.None:
                 emptyGun.gameObject.SetActive(true);

                 break;
             
             case EquipmentType.Backpack:
                 ak47.gameObject.SetActive(true);
                
                 break;
          
             case EquipmentType.Helmet:
                 m3.gameObject.SetActive(true);

                 break;
            
            
             case EquipmentType.Vest:
                  m3.gameObject.SetActive(true);

                  break;
            
              default:
                  break;
        }
        
        currentEquipment = equipmentType;
    }


  

}
