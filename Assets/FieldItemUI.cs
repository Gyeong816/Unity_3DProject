using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class FieldItemUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item Info")]
    public int itemId;
    public string itemName;
    public int itemPrice;
    public string itemUuId;

    [Header("UI")]
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;

    [Header("References")]
    public InventoryPanel inventoryPanel;
    public Lootbox lootboxOrigin;
    public LootBoxInventory LootBoxInventory; 



    public bool stashSlot = false;

    public Transform originalParent;
    public Vector2 originalPosition;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rt;

    public Slot currentSlot;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();

        canvasGroup.blocksRaycasts = true;
    }

    void Start()
    {
        if (canvas == null)
            canvas = GetComponentInParent<Canvas>();
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        
        currentSlot = GetComponentInParent<Slot>();
        if (currentSlot != null)
            currentSlot.isTaken = true;

        if (string.IsNullOrEmpty(itemUuId))
            itemUuId = Guid.NewGuid().ToString();

        if (nameText != null)
            nameText.text = itemName;
        if (priceText != null)
            priceText.text = $"${itemPrice}";
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;

        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
        FreeOccupiedSlots();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject target = eventData.pointerEnter;

        // 인벤토리 슬롯에 담기
        if (target != null && target.CompareTag("PlayerSlot"))
        {
            Slot slot = target.GetComponent<Slot>();
            if (slot != null && CanPlaceItem(slot))
            {
                PlaceInSlot(slot);

                // 필드 → 인벤토리 이동 시 원본 제거
                lootboxOrigin?.RemoveItemByUUID(itemUuId);
                LootBoxInventory?.RemoveSpawnedItem(gameObject);

                // 인벤토리 저장
                var data = new InventoryItemSaveData {
                    itemId      = itemName,
                    itemUuId    = itemUuId,
                    itemWidth   = 1,
                    itemHeight  = 1,
                    isRotated   = false,
                    x           = slot.x,
                    y           = slot.y,
                    isFieldItem = true,
                    itemPrice = itemPrice 
                };
                InventoryManager.Instance.RegisterItem(data);
                stashSlot = false;
            }
            else ReturnItem();
        }
        // 스태쉬 슬롯에 저장
        else if (target != null && target.CompareTag("StashSlot"))
        {
            Slot slot = target.GetComponent<Slot>();
            if (slot != null && CanPlaceItem(slot))
            {
                PlaceInSlot(slot);

                // 기존 인벤토리에서 제거
                InventoryManager.Instance.UnregisterItem(itemUuId);

                var data = new InventoryItemSaveData {
                    itemId      = itemName,
                    itemUuId    = itemUuId,
                    itemWidth   = 1,
                    itemHeight  = 1,
                    isRotated   = false,
                    x           = slot.x,
                    y           = slot.y,
                    isFieldItem = true,
                    itemPrice = itemPrice
                };
                InventoryManager.Instance.RegisterStashItem(data);
                stashSlot = true;
            }
            else ReturnItem();
        }
        // 상점 슬롯에 판매
        else if (target != null && target.CompareTag("ShopSlot") && stashSlot)
        {
            TradeManager.Instance.ShowConfirm(
                true,
                itemName,
                itemPrice,
                () =>
                {
                    if (TradeManager.Instance.CanVendorAfford(itemPrice))
                    {
                        TradeManager.Instance.AddPlayerDoller(itemPrice);
                        TradeManager.Instance.SubtractVendorDoller(itemPrice);
                        InventoryManager.Instance.UnregisterStashItem(itemUuId);
                        Destroy(gameObject);
                    }
                    else
                    {
                        TradeManager.Instance.ShowWarningPanel();
                        ReturnItem();
                    }
                },
                () => ReturnItem()
            );
        }
        else
        {
            ReturnItem();
        }

        canvasGroup.blocksRaycasts = true;
    }

    private void PlaceInSlot(Slot slot)
    {
        transform.SetParent(slot.transform);
        transform.position = slot.transform.position;
        slot.isTaken = true;
        currentSlot = slot;
    }

    private void ReturnItem()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        currentSlot = originalParent.GetComponent<Slot>();
        if (currentSlot != null)
            currentSlot.isTaken = true;
    }

    private void FreeOccupiedSlots()
    {
        if (currentSlot != null)
            currentSlot.isTaken = false;
    }

    private bool CanPlaceItem(Slot slot)
    {
        return !slot.isTaken;
    }

    public void Init(InventoryPanel inventory, Lootbox origin, LootBoxInventory lootBoxInventory)
    {
        inventoryPanel = inventory;
        lootboxOrigin  = origin;
        LootBoxInventory = lootBoxInventory;
    }
}
