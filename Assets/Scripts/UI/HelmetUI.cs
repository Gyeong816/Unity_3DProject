using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelmetUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item Info")]
    public int itemWidth = 2;
    public int itemHeight = 2;
    public string itemId;
    public string itemUuId;
    public int itemPrice = 100;

    public PlayerEquipment.EquipmentType equipmentType;
    private PlayerEquipment.EquipmentType noneHelmet = PlayerEquipment.EquipmentType.NONEHELMET;

    [Header("Pivot Settings")]
    public float pivotX = 0.75f;
    public float pivotY = 0.25f;
    public float rotatedPivotY = 0.75f;

    [Header("References")]
    public InventoryPanel inventoryPanel;
    public PlayerEquipment playerEquipment;

    public Image itemImage;
    private Image backgroundImage;
    private RectTransform rt;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private Transform originalParent;
    private Vector2 originalPosition;
    private bool isDragging = false;
    public bool isRotated = false;
    private bool originalRotation = false;

    public bool helmetSlot = false;
    private bool enemyHelmetSlot = false;
    private bool canPlace = false;

    public bool shopSlot = false;
    public bool stashSlot = false;

    private List<Slot> occupiedSlots = new List<Slot>();

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        backgroundImage = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            isRotated = !isRotated;
            rt.pivot = new Vector2(pivotX, isRotated ? rotatedPivotY : pivotY);
            backgroundImage.rectTransform.localEulerAngles = isRotated
                ? new Vector3(0, 0, -90f)
                : Vector3.zero;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = isRotated;

        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;
        FreeOccupiedSlots();

        if (helmetSlot || enemyHelmetSlot)
        {
            rt.pivot = new Vector2(pivotX, pivotY);
            int width = isRotated ? itemHeight : itemWidth;
            int height = isRotated ? itemWidth : itemHeight;
            rt.sizeDelta = new Vector2(width * 100f, height * 100f);
            itemImage.rectTransform.sizeDelta = new Vector2(180f, 180f);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        GameObject target = eventData.pointerEnter;
        RectTransform itemRect = rt;

        if (target != null)
        {
            // 1) Drop into player inventory
            if (target.CompareTag("PlayerSlot"))
            {
                Slot slot = target.GetComponent<Slot>();
                if (slot == null)
                {
                    ReturnItem();
                    canvasGroup.blocksRaycasts = true;
                    return;
                }

                inventoryPanel = slot.GetComponentInParent<InventoryPanel>() ?? inventoryPanel;
                CanPlaceItem(slot.x, slot.y);

                if (canPlace)
                {
                    transform.SetParent(slot.transform);
                    transform.position = slot.transform.position;
                    OccupySlots(slot.x, slot.y);

                    if (helmetSlot)
                    {
                        itemRect.sizeDelta = new Vector2(itemWidth * 100f, itemHeight * 100f);
                        rt.pivot = new Vector2(pivotX, pivotY);
                        helmetSlot = false;
                        playerEquipment?.SetEquipment(noneHelmet);
                    }
                    else if (enemyHelmetSlot)
                    {
                        rt.pivot = new Vector2(pivotX, pivotY);
                        enemyHelmetSlot = false;
                    }

                    slot.isTaken = true;
                    InventoryManager.Instance.UnregisterStashItem(itemUuId);

                    var data = new InventoryItemSaveData {
                        itemId = itemId,
                        x = slot.x,
                        y = slot.y,
                        isRotated = isRotated,
                        itemWidth = itemWidth,
                        itemHeight = itemHeight,
                        equipmentType = equipmentType,
                        itemPrice = itemPrice,
                        itemUuId = itemUuId
                    };
                    InventoryManager.Instance.RegisterItem(data);
                }
                else
                {
                    ReturnItem();
                }
            }
            // 2) Drop into stash
            else if (target.CompareTag("StashSlot"))
            {
                Slot slot = target.GetComponent<Slot>();
                if (slot == null)
                {
                    ReturnItem();
                    canvasGroup.blocksRaycasts = true;
                    return;
                }

                inventoryPanel = slot.GetComponentInParent<InventoryPanel>() ?? inventoryPanel;
                CanPlaceItem(slot.x, slot.y);

                // A) Normal stash move
                if (!shopSlot && canPlace)
                {
                    transform.SetParent(slot.transform);
                    transform.position = slot.transform.position;
                    OccupySlots(slot.x, slot.y);
                    slot.isTaken = true;
                    stashSlot = true;

                    InventoryManager.Instance.UnregisterItem(itemUuId);
                    var data = new InventoryItemSaveData {
                        itemId = itemId,
                        x = slot.x,
                        y = slot.y,
                        isRotated = isRotated,
                        itemWidth = itemWidth,
                        itemHeight = itemHeight,
                        equipmentType = equipmentType,
                        itemPrice = itemPrice,
                        itemUuId = itemUuId
                    };
                    InventoryManager.Instance.RegisterStashItem(data);
                }
                // B) Purchase into stash
                else if (shopSlot && canPlace)
                {
                    TradeManager.Instance.ShowConfirm(
                        false,
                        itemId,
                        itemPrice,
                        () =>
                        {
                            if (TradeManager.Instance.CanPlayerAfford(itemPrice))
                            {
                                TradeManager.Instance.SubtractPlayerDoller(itemPrice);
                                TradeManager.Instance.AddVendorDoller(itemPrice);

                                transform.SetParent(slot.transform);
                                transform.position = slot.transform.position;
                                OccupySlots(slot.x, slot.y);
                                slot.isTaken = true;
                                stashSlot = true;

                                if (string.IsNullOrEmpty(itemUuId))
                                    itemUuId = Guid.NewGuid().ToString();

                                var data = new InventoryItemSaveData {
                                    itemId = itemId,
                                    x = slot.x,
                                    y = slot.y,
                                    isRotated = isRotated,
                                    itemWidth = itemWidth,
                                    itemHeight = itemHeight,
                                    equipmentType = equipmentType,
                                    itemPrice = itemPrice,
                                    itemUuId = itemUuId
                                };
                                InventoryManager.Instance.RegisterStashItem(data);
                            }
                            else
                            {
                                TradeManager.Instance.ShowWarningPanel();
                                ReturnItem();
                            }
                        },
                        () => { ReturnItem(); }
                    );
                }
                // C) Fallback stash (identical to normal)
                else if (!shopSlot && canPlace)
                {
                    transform.SetParent(slot.transform);
                    transform.position = slot.transform.position;
                    OccupySlots(slot.x, slot.y);
                    slot.isTaken = true;

                    InventoryManager.Instance.UnregisterItem(itemUuId);
                    var data = new InventoryItemSaveData {
                        itemId = itemId,
                        x = slot.x,
                        y = slot.y,
                        isRotated = isRotated,
                        itemWidth = itemWidth,
                        itemHeight = itemHeight,
                        equipmentType = equipmentType,
                        itemPrice = itemPrice,
                        itemUuId = itemUuId
                    };
                    InventoryManager.Instance.RegisterStashItem(data);
                }
                else
                {
                    ReturnItem();
                }
            }
            // 3) Sell from stash to shop
            else if (target.CompareTag("ShopSlot") && stashSlot)
            {
                TradeManager.Instance.ShowConfirm(
                    true,
                    itemId,
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
                    () => { ReturnItem(); }
                );
            }
            // 4) Equip to character
            else if (target.CompareTag("HelmetSlot"))
            {
                helmetSlot = true;
                EquipItem(target, itemRect);

                playerEquipment?.SetEquipment(equipmentType);
                GameData.Instance.selectedHelmetType = equipmentType;

                InventoryManager.Instance.UnregisterItem(itemUuId);
                InventoryManager.Instance.UnregisterStashItem(itemUuId);

        

                RectTransform slotRect = target.GetComponent<RectTransform>();
                if (slotRect != null)
                {
                    rt.sizeDelta = slotRect.sizeDelta;
                    rt.pivot = new Vector2(0.5f, 0.5f);
                    itemImage.rectTransform.sizeDelta = slotRect.sizeDelta;
                }
            }
            // 5) Delete
            else if (target.CompareTag("DeleteSlot"))
            {
                if (helmetSlot)
                {
                    playerEquipment.SetEquipment(noneHelmet);
                    GameData.Instance.selectedVestType = PlayerEquipment.EquipmentType.NONEHELMET;
                }
                InventoryManager.Instance.UnregisterItem(itemUuId);
                InventoryManager.Instance.UnregisterStashItem(itemUuId);
                Destroy(gameObject);
            }
            else
            {
                ReturnItem();
            }
        }
        else
        {
            ReturnItem();
        }

        canvasGroup.blocksRaycasts = true;
    }

    private void EquipItem(GameObject target, RectTransform itemRect)
    {
        transform.SetParent(target.transform);
        transform.position = target.transform.position;
        rt.pivot = new Vector2(0.5f, 0.5f);
        itemRect.sizeDelta = target.GetComponent<RectTransform>().sizeDelta;
    }

    private void ReturnItem()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;

        foreach (var slot in occupiedSlots)
            slot.isTaken = true;

        isRotated = originalRotation;

        if (isRotated)
        {
            rt.pivot = new Vector2(pivotX, rotatedPivotY);
            backgroundImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90f);
        }
        else
        {
            rt.pivot = new Vector2(pivotX, pivotY);
            backgroundImage.rectTransform.localEulerAngles = Vector3.zero;
        }

        if (helmetSlot || enemyHelmetSlot)
            rt.pivot = new Vector2(0.5f, 0.5f);
    }

    private void CanPlaceItem(int startX, int startY)
    {
        canPlace = true;
        int width = isRotated ? itemHeight : itemWidth;
        int height = isRotated ? itemWidth : itemHeight;

        for (int x = 0; x < width && canPlace; x++)
            for (int y = 0; y < height; y++)
            {
                int tx = startX - x, ty = startY - y;
                if (tx < 0 || ty < 0 ||
                    tx >= inventoryPanel.columns || ty >= inventoryPanel.rows ||
                    inventoryPanel.slots[tx, ty].isTaken)
                {
                    canPlace = false;
                    return;
                }
            }
    }

    public void OccupySlots(int startX, int startY)
    {
        occupiedSlots.Clear();

        int width = isRotated ? itemHeight : itemWidth;
        int height = isRotated ? itemWidth : itemHeight;

        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var slot = inventoryPanel.slots[startX - x, startY - y];
                slot.isTaken = true;
                occupiedSlots.Add(slot);
            }
    }

    private void FreeOccupiedSlots()
    {
        foreach (var slot in occupiedSlots)
            slot.isTaken = false;
        occupiedSlots.Clear();
    }

    public void Init(InventoryPanel inventory, PlayerEquipment equipment, bool EnemyHelmetSlot = false)
    {
        inventoryPanel = inventory;
        playerEquipment = equipment;
        enemyHelmetSlot = EnemyHelmetSlot;
    }
}
