using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItemSaveData
{
    public string itemId;
    public string itemUuId; 
    public int x;
    public int y;
    public bool isRotated;
    public int itemPrice;
    
    public int itemWidth;
    public int itemHeight;
    public PlayerWeapon.WeaponType weaponType;
    public PlayerWeapon.BulletType bulletType;
    public PlayerEquipment.EquipmentType equipmentType;
    
    public bool isFieldItem;

    public int bulletAmount;
    public int healAmount;

}