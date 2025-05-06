using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInventory : MonoBehaviour
{
    [Header("References")]
    public InventoryPanel inventoryPanel;
    public PlayerWeapon playerWeapon;
    public PlayerEquipment playerEquipment;

    public EnemyController currentEnemy;

    public Image vestSlot;
    public Image weaponSlot;
    public Image helmetSlot;

    [Header("Item Prefabs")]
    public GameObject vest1Prefab;
    public GameObject vest3Prefab;
    public GameObject gunPrefab;
    public GameObject helmet1Prefab;


    private GameObject spawnedVest;
    private GameObject spawnedGun;
    private GameObject spawnedHelmet;

    private void Update()
    {
        if (spawnedVest != null)
        {
            if (spawnedVest.transform.parent != vestSlot.transform)
            {
 

                currentEnemy.UneqiupVest();

                spawnedVest = null; 
            }
        }
        if (spawnedGun != null)
        {
            if (spawnedGun.transform.parent != weaponSlot.transform)
            {
 

                currentEnemy.UneqiupAk47();

                spawnedGun = null; 
            }
        }
        if (spawnedHelmet != null)
        {
            if (spawnedHelmet.transform.parent != helmetSlot.transform)
            {
 

                currentEnemy.UneqiupHelmet();

                spawnedGun = null; 
            }
        }
    }


    public void SpawnVest1()
    {
        spawnedVest = Instantiate(vest1Prefab, inventoryPanel.transform);

        var vestUI = spawnedVest.GetComponent<VestUI>();
        vestUI.Init(inventoryPanel,playerEquipment, true);

        RectTransform parent = vestSlot.rectTransform;
        RectTransform rt = spawnedVest.GetComponent<RectTransform>();

        spawnedVest.transform.SetParent(vestSlot.transform);

        string generatedUuId = Guid.NewGuid().ToString();
        vestUI.itemUuId = generatedUuId;

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = parent.sizeDelta;
        vestUI.itemImage.rectTransform.sizeDelta = parent.sizeDelta;
    }
    public void SpawnVest3()
    {
        spawnedVest = Instantiate(vest3Prefab, inventoryPanel.transform);

        var vestUI = spawnedVest.GetComponent<VestUI>();
        vestUI.Init(inventoryPanel, playerEquipment, true);

        RectTransform parent = vestSlot.rectTransform;
        RectTransform rt = spawnedVest.GetComponent<RectTransform>();

        spawnedVest.transform.SetParent(vestSlot.transform);

        string generatedUuId = Guid.NewGuid().ToString();
        vestUI.itemUuId = generatedUuId;

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = parent.sizeDelta;
        vestUI.itemImage.rectTransform.sizeDelta = parent.sizeDelta;

       
    }
    
    public void SpawnHelmet1()
    {
        spawnedHelmet = Instantiate(helmet1Prefab, inventoryPanel.transform);

        var helmetUI = spawnedHelmet.GetComponent<HelmetUI>();
        helmetUI.Init(inventoryPanel, playerEquipment, true);

        RectTransform parent = helmetSlot.rectTransform;
        RectTransform rt = spawnedHelmet.GetComponent<RectTransform>();

        spawnedHelmet.transform.SetParent(helmetSlot.transform);

        string generatedUuId = Guid.NewGuid().ToString();
        helmetUI.itemUuId = generatedUuId;

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = parent.sizeDelta;
        helmetUI.itemImage.rectTransform.sizeDelta = parent.sizeDelta;


    }
    
    public void SpawnGun()
    {
        spawnedGun = Instantiate(gunPrefab, inventoryPanel.transform);

        var gunUI = spawnedGun.GetComponent<WeaponUI>();
        gunUI.Init(inventoryPanel, playerWeapon, true);

        RectTransform parent = weaponSlot.rectTransform;
        RectTransform rt = spawnedGun.GetComponent<RectTransform>();

        spawnedGun.transform.SetParent(weaponSlot.transform);

        string generatedUuId = Guid.NewGuid().ToString();
        gunUI.itemUuId = generatedUuId;

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = parent.sizeDelta;
        gunUI.itemImage.rectTransform.sizeDelta = parent.sizeDelta;
    }

    public void SetEnemy(EnemyController CurrentEnemy)
    {
        currentEnemy = CurrentEnemy;
    }


}
