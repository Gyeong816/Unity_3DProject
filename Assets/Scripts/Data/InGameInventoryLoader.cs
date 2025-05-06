using System;
using System.Collections.Generic;
using UnityEngine;

public class InGameInventoryLoader : MonoBehaviour
{
    [Header("Inventory Panels")]
    public InventoryPanel playerInventoryPanel;
    public InventoryPanel lootboxInventoryPanel;

    [Header("Player References")]
    public PlayerWeapon playerWeapon;
    public PlayerEquipment playerEquipment;
    public PlayerHP playerHP;

    [Header("Equipment Slots")]
    public Transform weaponSlot;
    public Transform helmetSlot;
    public Transform vestSlot;


    void Start()
    {
        playerInventoryPanel.Init();
        lootboxInventoryPanel.Init();

        // 1) 장착된 장비 로드
        LoadEquippedSlot<WeaponUI, PlayerWeapon.WeaponType>(
            GameData.Instance?.selectedWeaponType ?? PlayerWeapon.WeaponType.NONE,
            PlayerWeapon.WeaponType.NONE,
            weaponSlot,
            playerInventoryPanel,
            (ui, id) =>
            {
                ui.itemId         = id;
                ui.weaponType     = GameData.Instance.selectedWeaponType;
                ui.inventoryPanel = playerInventoryPanel;
                ui.playerWeapon   = playerWeapon;
                ui.weaponSlot     = true;
            }
        );
        LoadEquippedSlot<HelmetUI, PlayerEquipment.EquipmentType>(
            GameData.Instance?.selectedHelmetType ?? PlayerEquipment.EquipmentType.NONEHELMET,
            PlayerEquipment.EquipmentType.NONEHELMET,
            helmetSlot,
            playerInventoryPanel,
            (ui, id) =>
            {
                ui.itemId           = id;
                ui.equipmentType    = GameData.Instance.selectedHelmetType;
                ui.inventoryPanel   = playerInventoryPanel;
                ui.playerEquipment  = playerEquipment;
                ui.helmetSlot       = true;
            }
        );
        LoadEquippedSlot<VestUI, PlayerEquipment.EquipmentType>(
            GameData.Instance?.selectedVestType ?? PlayerEquipment.EquipmentType.NONEVEST,
            PlayerEquipment.EquipmentType.NONEVEST,
            vestSlot,
            playerInventoryPanel,
            (ui, id) =>
            {
                ui.itemId           = id;
                ui.equipmentType    = GameData.Instance.selectedVestType;
                ui.inventoryPanel   = playerInventoryPanel;
                ui.playerEquipment  = playerEquipment;
                ui.vestSlot         = true;
            }
        );

        // 2) 플레이어 인벤토리 아이템 로드
        LoadPanelItems(InventoryManager.Instance.savedItems, playerInventoryPanel);
        // 3) 룻박스 아이템 로드 (LootBoxInventory가 저장한 데이터가 있다면)
    }

    /// <summary>
    /// 장착 슬롯용 제네릭 헬퍼
    /// </summary>
    private void LoadEquippedSlot<TUI, TEnum>(
        TEnum selected,
        TEnum noneValue,
        Transform slotTransform,
        InventoryPanel panel,
        Action<TUI, string> initialize
    )
        where TUI : MonoBehaviour
    {
        if (EqualityComparer<TEnum>.Default.Equals(selected, noneValue))
            return;

        string itemId = selected.ToString();
        var prefab = ItemDataManager.Instance.GetItemUIPrefabById(itemId);
        if (prefab == null)
        {
            Debug.LogWarning($"Prefab not found: {itemId}");
            return;
        }

        var go = Instantiate(prefab, slotTransform);
        if (go.TryGetComponent<TUI>(out var ui))
        {
            initialize(ui, itemId);
            AlignToSlot(go.transform, slotTransform);

            var slotRect = slotTransform.GetComponent<RectTransform>();
            var itemRect = go.GetComponent<RectTransform>();
            ResizeToSlot(itemRect, slotRect);

            // 자식 이미지도 슬롯 크기에 맞춤
            if (ui is WeaponUI w) w.itemImage.rectTransform.sizeDelta = slotRect.sizeDelta;
            else if (ui is HelmetUI h) h.itemImage.rectTransform.sizeDelta = slotRect.sizeDelta;
            else if (ui is VestUI v)   v.itemImage.rectTransform.sizeDelta = slotRect.sizeDelta;
        }
    }

    /// <summary>
    /// IReadOnlyList<T> 를 받아 슬롯에 아이템 배치
    /// </summary>
    private void LoadPanelItems(IReadOnlyList<InventoryItemSaveData> list, InventoryPanel panel)
    {
        if (list == null || list.Count == 0) return;

        foreach (var data in list)
        {
            GameObject prefab = data.isFieldItem
                ? ItemDataManager.Instance.GetFieldItemUIPrefab()
                : ItemDataManager.Instance.GetItemUIPrefabById(data.itemId);

            if (prefab == null)
            {
                Debug.LogWarning($"Prefab missing: {data.itemId}");
                continue;
            }

            var go = Instantiate(prefab);
            PlaceAndInitialize(go, data, panel);
        }
    }

    /// <summary>
    /// 슬롯에 배치하고 해당 UI 컴포넌트를 초기화
    /// </summary>
    private void PlaceAndInitialize(GameObject go, InventoryItemSaveData data, InventoryPanel panel)
    {
        var slot = panel.slots[data.x, data.y];
        go.transform.SetParent(slot.transform, false);

        // WeaponUI
        if (go.TryGetComponent<WeaponUI>(out var w))
        {
            w.itemId = data.itemId;
            w.itemUuId = data.itemUuId;
            w.isRotated = data.isRotated;
            w.itemWidth = data.itemWidth;
            w.itemHeight = data.itemHeight;
            w.weaponType = data.weaponType;
            w.itemPrice = data.itemPrice; // 가격 추가
            w.inventoryPanel = panel;
            w.playerWeapon = playerWeapon;
            if (w.isRotated) w.ApplyRotation();
            w.OccupySlots(data.x, data.y);
            return;
        }

        // HelmetUI
        if (go.TryGetComponent<HelmetUI>(out var h))
        {
            h.itemId = data.itemId;
            h.itemUuId = data.itemUuId;
            h.isRotated = data.isRotated;
            h.itemWidth = data.itemWidth;
            h.itemHeight = data.itemHeight;
            h.equipmentType = data.equipmentType;
            h.itemPrice = data.itemPrice; // 가격 추가
            h.inventoryPanel = panel;
            h.playerEquipment = playerEquipment;
            h.OccupySlots(data.x, data.y);
            return;
        }

        // VestUI
        if (go.TryGetComponent<VestUI>(out var v))
        {
            v.itemId = data.itemId;
            v.itemUuId = data.itemUuId;
            v.isRotated = data.isRotated;
            v.itemWidth = data.itemWidth;
            v.itemHeight = data.itemHeight;
            v.equipmentType = data.equipmentType;
            v.itemPrice = data.itemPrice; // 가격 추가
            v.inventoryPanel = panel;
            v.playerEquipment = playerEquipment;
            if (v.isRotated) v.ApplyRotation();
            v.OccupySlots(data.x, data.y);
            return;
        }

        // BulletUI
        if (go.TryGetComponent<BulletUI>(out var b))
        {
            b.itemId = data.itemId;
            b.itemUuId = data.itemUuId;
            b.bulletAmount = data.bulletAmount;
            b.bulletType = data.bulletType; 
            b.itemPrice = data.itemPrice;  
            b.inventoryPanel = panel;
            b.playerWeapon = playerWeapon;
            b.currentSlot = slot;
            b.originalParent = slot.transform;
            b.originalPosition = slot.transform.position;
            slot.isTaken = true;

            if (b.ammoText != null)
                b.ammoText.text = b.bulletAmount.ToString();

            // 위치 정렬
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.localRotation = Quaternion.identity;
            rect.localScale = Vector3.one;

            return;
        }

        // FieldItemUI
        if (go.TryGetComponent<FieldItemUI>(out var f))
        {
            f.itemName = data.itemId;
            f.itemUuId = data.itemUuId;
            f.itemPrice = data.itemPrice;
            f.inventoryPanel = panel;

            f.nameText.text = data.itemId;
            f.priceText.text = $"${data.itemPrice}";

            f.currentSlot = slot;
            f.originalParent = slot.transform;
            f.originalPosition = slot.transform.position;

            slot.isTaken = true;

            // 위치 정렬
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
            m.playerHP = playerHP;      

            m.currentSlot = slot;
            m.originalParent = slot.transform;
            m.originalPosition = slot.transform.position;

            slot.isTaken = true;

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

    public void ClearPlayerInventory()
    {
        // 슬롯에 있는 아이템 제거
        foreach (Slot slot in playerInventoryPanel.slots)
        {
            if (slot.transform.childCount > 0)
            {
                GameObject child = slot.transform.GetChild(0).gameObject;
                Destroy(child);
                slot.isTaken = false;
            }
        }

        // 선택된 장비 초기화
        GameData.Instance.selectedWeaponType = PlayerWeapon.WeaponType.NONE;
        GameData.Instance.selectedHelmetType = PlayerEquipment.EquipmentType.NONEHELMET;
        GameData.Instance.selectedVestType = PlayerEquipment.EquipmentType.NONEVEST;

        // 인벤토리 데이터 초기화
        InventoryManager.Instance.savedItems.Clear();
    }
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
