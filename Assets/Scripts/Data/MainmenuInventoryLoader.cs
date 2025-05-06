using System;
using System.Collections.Generic;
using UnityEngine;

public class MainmenuInventoryLoader : MonoBehaviour
{
    [Header("Inventory Panels")]
    public InventoryPanel playerInventoryPanel;
    public InventoryPanel stashInventoryPanel;

    [Header("Equipment Slots")]
    public Transform weaponSlot;
    public Transform helmetSlot;
    public Transform vestSlot;

    void Start()
    {
        InventoryManager.Instance.LoadFromFile();
        Debug.Log($"[MainmenuInventoryLoader] Loaded → WeaponType: {GameData.Instance.selectedWeaponType}");

        playerInventoryPanel.Init();
        stashInventoryPanel.Init();


        if (GameData.Instance.selectedWeaponType != PlayerWeapon.WeaponType.NONE)
            LoadEquippedWeapon();

        if (GameData.Instance.selectedHelmetType != PlayerEquipment.EquipmentType.NONEHELMET)
            LoadEquippedHelmet();

        if (GameData.Instance.selectedVestType != PlayerEquipment.EquipmentType.NONEVEST)
            LoadEquippedVest();

        LoadPanelItems(InventoryManager.Instance.savedItems,  playerInventoryPanel);
        LoadPanelItems(InventoryManager.Instance.stashSavedItems, stashInventoryPanel);
    }

    #region Equipped Load

    private void LoadEquippedWeapon()
    {
        var selected = GameData.Instance?.selectedWeaponType ?? PlayerWeapon.WeaponType.NONE;
        if (selected == PlayerWeapon.WeaponType.NONE) return;
        string itemId = selected.ToString();
        var prefab = ItemDataManager.Instance.GetItemUIPrefabById(itemId);
        if (prefab == null) { Debug.LogWarning($"Weapon prefab not found: {itemId}"); return; }
        var go = Instantiate(prefab, weaponSlot);
        var ui = go.GetComponent<WeaponUI>();
        if (ui == null) return;
        ui.itemId         = itemId;
        ui.weaponType     = selected;
        ui.itemUuId       = null;
        ui.inventoryPanel = playerInventoryPanel;
        ui.playerWeapon   = null;
        ui.weaponSlot     = true;
        AlignToSlot(go.transform, weaponSlot);
        var slotRect = weaponSlot.GetComponent<RectTransform>();
        var itemRect = ui.GetComponent<RectTransform>();
        ResizeToSlot(itemRect, slotRect);
        ui.itemImage.rectTransform.sizeDelta = slotRect.sizeDelta;
    }

    private void LoadEquippedHelmet()
    {
        var selected = GameData.Instance?.selectedHelmetType ?? PlayerEquipment.EquipmentType.NONEHELMET;
        if (selected == PlayerEquipment.EquipmentType.NONEHELMET) return;
        string itemId = selected.ToString();
        var prefab = ItemDataManager.Instance.GetItemUIPrefabById(itemId);
        if (prefab == null) { Debug.LogWarning($"Helmet prefab not found: {itemId}"); return; }
        var go = Instantiate(prefab, helmetSlot);
        var ui = go.GetComponent<HelmetUI>();
        if (ui == null) return;
        ui.itemId         = itemId;
        ui.equipmentType  = selected;
        ui.itemUuId       = null;
        ui.inventoryPanel = playerInventoryPanel;
        ui.playerEquipment= null;
        ui.helmetSlot     = true;
        AlignToSlot(go.transform, helmetSlot);
        var slotRect = helmetSlot.GetComponent<RectTransform>();
        var itemRect = ui.GetComponent<RectTransform>();
        ResizeToSlot(itemRect, slotRect);
        ui.itemImage.rectTransform.sizeDelta = slotRect.sizeDelta;
    }

    private void LoadEquippedVest()
    {
        var selected = GameData.Instance?.selectedVestType ?? PlayerEquipment.EquipmentType.NONEVEST;
        if (selected == PlayerEquipment.EquipmentType.NONEVEST) return;
        string itemId = selected.ToString();
        var prefab = ItemDataManager.Instance.GetItemUIPrefabById(itemId);
        if (prefab == null) { Debug.LogWarning($"Vest prefab not found: {itemId}"); return; }
        var go = Instantiate(prefab, vestSlot);
        var ui = go.GetComponent<VestUI>();
        if (ui == null) return;
        ui.itemId         = itemId;
        ui.equipmentType  = selected;
        ui.itemUuId       = null;
        ui.inventoryPanel = playerInventoryPanel;
        ui.playerEquipment= null;
        ui.vestSlot       = true;
        AlignToSlot(go.transform, vestSlot);
        var slotRect = vestSlot.GetComponent<RectTransform>();
        var itemRect = ui.GetComponent<RectTransform>();
        ResizeToSlot(itemRect, slotRect);
        ui.itemImage.rectTransform.sizeDelta = slotRect.sizeDelta;
    }

    #endregion

    #region Inventory & Stash Load

    private void LoadPanelItems(List<InventoryItemSaveData> list, InventoryPanel panel)
    {
        if (list == null || list.Count == 0) return;

        foreach (var data in list)
        {
            GameObject prefab = data.isFieldItem
                ? ItemDataManager.Instance.GetFieldItemUIPrefab()
                : ItemDataManager.Instance.GetItemUIPrefabById(data.itemId);

            if (prefab == null)
            {
                Debug.LogWarning($"Item prefab missing: {data.itemId}");
                continue;
            }

            var go = Instantiate(prefab);
            PlaceAndInitialize(go, data, panel);
        }
    }

    private void PlaceAndInitialize(GameObject go, InventoryItemSaveData data, InventoryPanel panel)
    {
        // 1) 슬롯 위에 자식으로 배치
        Slot slot = panel.slots[data.x, data.y];
        go.transform.SetParent(slot.transform, false);

        bool isStash = panel == stashInventoryPanel;
        
        // 2) 공통 UI 프로퍼티 설정
        if (go.TryGetComponent<WeaponUI>(out var w))
        {
            w.itemId         = data.itemId;
            w.itemUuId       = data.itemUuId;
            w.isRotated      = data.isRotated;
            w.itemWidth      = data.itemWidth;
            w.itemHeight     = data.itemHeight;
            w.weaponType     = data.weaponType;
            w.inventoryPanel = panel;
            w.playerWeapon   = null;
            if (w.isRotated) w.ApplyRotation();
            w.OccupySlots(data.x, data.y);
            if (isStash) w.stashSlot = true;
            return;
        }
        if (go.TryGetComponent<HelmetUI>(out var h))
        {
            h.itemId         = data.itemId;
            h.itemUuId       = data.itemUuId;
            h.isRotated      = data.isRotated;
            h.itemWidth      = data.itemWidth;
            h.itemHeight     = data.itemHeight;
            h.equipmentType  = data.equipmentType;
            h.inventoryPanel = panel;
            h.playerEquipment= null;
            h.OccupySlots(data.x, data.y);
            if (isStash) h.stashSlot = true;
            return;
        }
        if (go.TryGetComponent<VestUI>(out var v))
        {
            v.itemId         = data.itemId;
            v.itemUuId       = data.itemUuId;
            v.isRotated      = data.isRotated;
            v.itemWidth      = data.itemWidth;
            v.itemHeight     = data.itemHeight;
            v.equipmentType  = data.equipmentType;
            v.inventoryPanel = panel;
            v.playerEquipment= null;
            if (v.isRotated) v.ApplyRotation();
            v.OccupySlots(data.x, data.y);
            if (isStash) v.stashSlot = true;
            return;
        }
        if (go.TryGetComponent<BulletUI>(out var b))
        {
            b.itemId = data.itemId;
            b.itemUuId = data.itemUuId;
            b.bulletAmount = data.bulletAmount;
            b.bulletType = data.bulletType;
            b.itemPrice = data.itemPrice;
            b.inventoryPanel = panel;
            b.playerWeapon = null;

            b.currentSlot = slot;
            b.originalParent = slot.transform;
            b.originalPosition = slot.transform.position;

            slot.isTaken = true;
            if (isStash) b.stashSlot = true;

            if (b.ammoText != null)
                b.ammoText.text = b.bulletAmount.ToString();

            // 위치만 정확히 정렬
            RectTransform bulletRect = go.GetComponent<RectTransform>();
            bulletRect.anchorMin = new Vector2(0.5f, 0.5f);
            bulletRect.anchorMax = new Vector2(0.5f, 0.5f);
            bulletRect.pivot = new Vector2(0.5f, 0.5f);
            bulletRect.anchoredPosition = Vector2.zero;
            bulletRect.localRotation = Quaternion.identity;
            bulletRect.localScale = Vector3.one;

            return;
        }

        if (go.TryGetComponent<FieldItemUI>(out var f))
        {
            f.itemName = data.itemId;
            f.itemUuId = data.itemUuId;
            f.itemPrice = data.itemPrice;
            f.inventoryPanel = panel;

            if (f.nameText != null)
                f.nameText.text = data.itemId;
            if (f.priceText != null)
                f.priceText.text = $"${data.itemPrice}";

            f.currentSlot = slot;
            f.originalParent = slot.transform;
            f.originalPosition = slot.transform.position;

            slot.isTaken = true;
            if (isStash) f.stashSlot = true;

            // 위치 정렬 처리
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            return;
        }
        if (go.TryGetComponent<MedkitUI>(out var m))
        {
            m.itemId = data.itemId;
            m.itemUuId = data.itemUuId;
            m.healAmount = data.healAmount;
            m.itemPrice = data.itemPrice;
            m.inventoryPanel = panel;

            m.currentSlot = slot;
            m.originalParent = slot.transform;
            m.originalPosition = slot.transform.position;

            slot.isTaken = true;
            if (isStash) m.stashSlot = true;

            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            return;
        }
    }

    #endregion

    #region Helpers

    private void AlignToSlot(Transform item, Transform slot)
    {
        item.localPosition = Vector3.zero;
        item.localRotation = Quaternion.identity;
        item.localScale    = Vector3.one;
    }

    private void ResizeToSlot(RectTransform itemRect, RectTransform slotRect)
    {
        itemRect.sizeDelta = slotRect.sizeDelta;
        itemRect.pivot     = new Vector2(0.5f, 0.5f);
    }

    #endregion
}
