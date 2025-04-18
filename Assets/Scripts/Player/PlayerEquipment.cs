using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerEquipment : MonoBehaviour
{
    [Header("PlayerEquipment")]
    public GameObject Helmet;
    public GameObject Vest;


    public enum EquipmentType 
    {
        NoneHelmet,
        NoneVest, 
        Helmet,
        Vest
    }





    private EquipmentType currentEquipment;
    
    
    
    private void Start()
    {
      
      SetEquipment(EquipmentType.NoneHelmet);
      SetEquipment(EquipmentType.NoneVest);

    }
    

    public void SetEquipment(EquipmentType equipmentType)
    {



        switch (equipmentType)
        {
           
             case EquipmentType.NoneHelmet:

                Helmet.SetActive(false);

                break;


             case EquipmentType.NoneVest:

                Vest.SetActive(false);

                break;

             case EquipmentType.Helmet:

                Helmet.SetActive(true);

                break;
            
            
             case EquipmentType.Vest:

                Vest.SetActive(true);

                break;
            
             default:
                 break;
        }
        
        currentEquipment = equipmentType;
    }


  

}
