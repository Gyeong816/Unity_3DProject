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

    public Enemy currentEnemy;

    public Image VestSlot;

    [Header("Item Prefabs")]
    public GameObject VestPrefab;


    private GameObject spawnedVest;


    private void Update()
    {
        if (spawnedVest != null)
        {
            if (spawnedVest.transform.parent != VestSlot.transform)
            {
 

                currentEnemy.UneqiupVest1();

                spawnedVest = null; 
            }
        }
    }


    public void SpawnVest1()
    {
        spawnedVest = Instantiate(VestPrefab, inventoryPanel.transform);

        var dragHandler = spawnedVest.GetComponent<ItemDragHandler>();
        dragHandler.Init(inventoryPanel, playerWeapon, playerEquipment, true);

        RectTransform parent = VestSlot.rectTransform;
        RectTransform rt = spawnedVest.GetComponent<RectTransform>();

        spawnedVest.transform.SetParent(VestSlot.transform);

        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = parent.sizeDelta;
    }

    public void SetEnemy(Enemy CurrentEnemy)
    {
        currentEnemy = CurrentEnemy;
    }


}
