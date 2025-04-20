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
    public GameObject Vest1;


    public enum EquipmentType 
    {
        NoneHelmet,
        NoneVest, 
        Helmet,
        Vest,
        Vest1
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
                Vest1.SetActive(false);

                break;

             case EquipmentType.Helmet:

                Helmet.SetActive(true);

                break;
            
            
             case EquipmentType.Vest:

                Vest.SetActive(true);

                break;

            case EquipmentType.Vest1:

                Vest1.SetActive(true);

                break;

            default:
                 break;
        }
        
        currentEquipment = equipmentType;
    }


  

}
