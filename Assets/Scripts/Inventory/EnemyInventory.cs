using System;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInventory : MonoBehaviour
{
    [Header("References")]
    public InventoryPanel inventoryPanel;
    public PlayerWeapon playerWeapon;
    public PlayerEquipment playerEquipment;

    public Image vestSlot;
    public Image weaponSlot;
    public Image helmetSlot;

    [Header("Item Prefabs")]
    public GameObject vest1Prefab;
    public GameObject vest3Prefab;
    public GameObject gunPrefab;
    public GameObject helmet1Prefab;

    private EnemyController currentEnemy;


    void Update()
    {

        if (currentEnemy == null) return;

        if (vestSlot.transform.childCount == 0 && currentEnemy.hasVest)
        {
            currentEnemy.hasVest = false;
            if (currentEnemy.vest != null)
                currentEnemy.vest.SetActive(false);
        }

        if (weaponSlot.transform.childCount == 0 && currentEnemy.hasGun)
        {
            currentEnemy.hasGun = false;
            if (currentEnemy.enemyAk47 != null)
                currentEnemy.enemyAk47.SetActive(false);
        }

        if (helmetSlot.transform.childCount == 0 && currentEnemy.hasHelmet)
        {
            currentEnemy.hasHelmet = false;
            if (currentEnemy.helmet != null)
                currentEnemy.helmet.SetActive(false);
        }
    }


    public void SetEnemy(EnemyController enemy)
    {
        currentEnemy = enemy;

        ClearInventory();

        if (currentEnemy.hasVest)
            SpawnVest();

        if (currentEnemy.hasGun)
            SpawnGun();

        if (currentEnemy.hasHelmet)
            SpawnHelmet();
    }

    private void ClearInventory()
    {
        foreach (Transform child in vestSlot.transform)
            Destroy(child.gameObject);
        foreach (Transform child in weaponSlot.transform)
            Destroy(child.gameObject);
        foreach (Transform child in helmetSlot.transform)
            Destroy(child.gameObject);
    }

    private void SpawnVest()
    {
        GameObject prefab = currentEnemy.enemyType == EnemyType.Enemy1 ? vest1Prefab : vest3Prefab;
        GameObject go = Instantiate(prefab, vestSlot.transform);

        var ui = go.GetComponent<VestUI>();
        ui.Init(inventoryPanel, playerEquipment, true);
        ui.itemUuId = Guid.NewGuid().ToString();

        ApplySlotTransform(go.GetComponent<RectTransform>(), vestSlot.rectTransform);
        ui.itemImage.rectTransform.sizeDelta = vestSlot.rectTransform.sizeDelta;
    }

    private void SpawnGun()
    {
        GameObject go = Instantiate(gunPrefab, weaponSlot.transform);

        var ui = go.GetComponent<WeaponUI>();
        ui.Init(inventoryPanel, playerWeapon, true);
        ui.itemUuId = Guid.NewGuid().ToString();

        ApplySlotTransform(go.GetComponent<RectTransform>(), weaponSlot.rectTransform);
        ui.itemImage.rectTransform.sizeDelta = weaponSlot.rectTransform.sizeDelta;
    }

    private void SpawnHelmet()
    {
        GameObject go = Instantiate(helmet1Prefab, helmetSlot.transform);

        var ui = go.GetComponent<HelmetUI>();
        ui.Init(inventoryPanel, playerEquipment, true);
        ui.itemUuId = Guid.NewGuid().ToString();

        ApplySlotTransform(go.GetComponent<RectTransform>(), helmetSlot.rectTransform);
        ui.itemImage.rectTransform.sizeDelta = helmetSlot.rectTransform.sizeDelta;
    }

    private void ApplySlotTransform(RectTransform target, RectTransform parent)
    {
        target.anchorMin = new Vector2(0.5f, 0.5f);
        target.anchorMax = new Vector2(0.5f, 0.5f);
        target.pivot = new Vector2(0.5f, 0.5f);
        target.anchoredPosition = Vector2.zero;
        target.sizeDelta = parent.sizeDelta;
    }
}
