using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerEquipment : MonoBehaviour
{
    [Header("PlayerEquipment")]
    public GameObject Helmet1;
    public GameObject Helmet2;
    
    
    public GameObject Vest1;
    public GameObject Vest2;
    public GameObject Vest3;

    public enum EquipmentType 
    {
        NONEHELMET,
        NONEVEST, 
        HELMET1,
        HELMET2,
        VEST1,
        VEST2,
        VEST3
    }





    private EquipmentType currentEquipment;
    
    
    
    private void Start()
    {
      
      SetEquipment(EquipmentType.NONEHELMET);
      SetEquipment(EquipmentType.NONEVEST);
      

      if (GameData.Instance.selectedHelmetType != PlayerEquipment.EquipmentType.NONEHELMET)
      {
          SetEquipment(GameData.Instance.selectedHelmetType); 
      }
      if (GameData.Instance.selectedHelmetType != PlayerEquipment.EquipmentType.NONEVEST)
      {
          SetEquipment(GameData.Instance.selectedVestType); 
      }

    }
    

    public void SetEquipment(EquipmentType equipmentType)
    {



        switch (equipmentType)
        {
           
             case EquipmentType.NONEHELMET:
                Helmet1.SetActive(false);
                Helmet2.SetActive(false);
                break;


             case EquipmentType.NONEVEST:
                 Vest1.SetActive(false);
                 Vest2.SetActive(false);
                 Vest3.SetActive(false);
                break;

             case EquipmentType.HELMET1:
                Helmet1.SetActive(true);
                break;
             
             case EquipmentType.HELMET2:
                 Helmet2.SetActive(true);
                 break;

             case EquipmentType.VEST1:
                 Vest1.SetActive(true);
                 break;
            
             case EquipmentType.VEST2:
                 Vest2.SetActive(true);
                 break;
            
             case EquipmentType.VEST3:
                 Vest3.SetActive(true);
                 break;
             default:
                 break;
        }
        
        currentEquipment = equipmentType;
    }


  

}
