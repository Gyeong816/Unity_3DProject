using System.Collections.Generic;
using UnityEngine;

public class LootBoxInventory : MonoBehaviour
{
    public InventoryPanel inventoryPanel;
    public GameObject fieldItemPrefab;

    private List<GameObject> spawnedItems = new();

    public void SetItems(List<FieldItemInfo> items, Lootbox originLootbox)
    {
        if (items == null || items.Count == 0)
        {
            return;
        }

        
        ClearItems();

        foreach (var item in items)
        {
            SpawnItem(item, originLootbox);
        }
    }

    private void ClearItems()
    {
        // 생성된 오브젝트 삭제
        foreach (var item in spawnedItems)
        {
            if (item != null)
                Destroy(item);
        }
        spawnedItems.Clear();

        // 슬롯 초기화
        foreach (var slot in inventoryPanel.slots)
        {
            slot.isTaken = false;
        }
    }

    private void SpawnItem(FieldItemInfo itemInfo, Lootbox originLootbox)
    {
        Slot[,] slots = inventoryPanel.slots;
        int targetX = itemInfo.slotX;
        int targetY = itemInfo.slotY;

        bool placed = false;

        // 1. slotX, slotY 지정된 경우 우선 시도
        if (IsValidSlot(targetX, targetY) && !slots[targetX, targetY].isTaken)
        {
            PlaceItem(slots[targetX, targetY], itemInfo, originLootbox);
            placed = true;
        }
        else
        {
            // 2. 지정 안 되어 있거나 자리가 찼으면, 빈 슬롯 자동 검색
            for (int y = 0; y < inventoryPanel.rows && !placed; y++)
            {
                for (int x = 0; x < inventoryPanel.columns && !placed; x++)
                {
                    if (!slots[x, y].isTaken)
                    {
                        itemInfo.slotX = x;
                        itemInfo.slotY = y;
                        PlaceItem(slots[x, y], itemInfo, originLootbox);
                        placed = true;
                    }
                }
            }
        }

        if (!placed)
        {
            Debug.LogWarning($"Lootbox 슬롯이 부족합니다. 아이템: {itemInfo.itemName}");
        }
    }

    private void PlaceItem(Slot slot, FieldItemInfo itemInfo, Lootbox originLootbox)
    {
        GameObject obj = Instantiate(fieldItemPrefab, slot.transform);
        var itemUI = obj.GetComponent<FieldItemUI>();
        itemUI.Init(inventoryPanel, originLootbox, this);
        
        itemUI.itemId = itemInfo.id;
        itemUI.itemName = itemInfo.itemName;
        itemUI.itemPrice = itemInfo.price;
        itemUI.itemUuId = itemInfo.itemUuId;
        itemUI.lootboxOrigin = originLootbox;

        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;

        slot.isTaken = true;

        spawnedItems.Add(obj);
    }

    private bool IsValidSlot(int x, int y)
    {
        return x >= 0 && y >= 0 && x < inventoryPanel.columns && y < inventoryPanel.rows;
    }
    
    public void RemoveSpawnedItem(GameObject item)
    {
        if (spawnedItems.Contains(item))
        {
            spawnedItems.Remove(item);
        }
    }
}
