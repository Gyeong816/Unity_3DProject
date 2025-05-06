using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VestUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Item Info")]
    public int itemWidth = 3;
    public int itemHeight = 4;
    public string itemUuId;
    public string itemId;
    public int itemPrice = 100;

    public PlayerEquipment.EquipmentType equipmentType;
    private PlayerEquipment.EquipmentType noneVest = PlayerEquipment.EquipmentType.NONEVEST;

    [Header("Pivot Settings")]
    public float pivotX = 0.83f;
    public float pivotY = 0.13f;
    public float rotatedPivotY = 0.87f;

    [Header("References")]
    public InventoryPanel inventoryPanel;
    public PlayerEquipment playerEquipment;

    private Transform originalParent;
    private Vector2 originalPosition;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rt;
    public Image itemImage;
    private Image backgroundImage;

    private bool isDragging = false;
    public bool isRotated = false;
    private bool originalRotation = false;

    public bool vestSlot = false;
    private bool enemyVestSlot = false;
    private bool canPlace = false;

    public bool shopSlot;
    public bool stashSlot;

    private List<Slot> occupiedSlots = new List<Slot>();

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
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

    public void ApplyRotation()
    {
        rt.pivot = new Vector2(pivotX, rotatedPivotY);
        backgroundImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90f);
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

        if (vestSlot || enemyVestSlot)
        {
            rt.pivot = new Vector2(pivotX, pivotY);
            int width = isRotated ? itemHeight : itemWidth;
            int height = isRotated ? itemWidth : itemHeight;
            rt.sizeDelta = new Vector2(width * 100f, height * 100f);
            itemImage.rectTransform.sizeDelta = new Vector2(280f, 380f);
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
        RectTransform itemRect = GetComponent<RectTransform>();

        if (target != null)
        {
            // Drop into Player Inventory
            if (target.CompareTag("PlayerSlot"))
            {
                Slot slot = target.GetComponent<Slot>();
                if (slot == null)
                {
                    ReturnItem();
                    canvasGroup.blocksRaycasts = true;
                    return;
                }

                InventoryPanel newPanel = slot.GetComponentInParent<InventoryPanel>();
                if (newPanel != null)
                    inventoryPanel = newPanel;

                CanPlaceItem(slot.x, slot.y);

                if (canPlace)
                {
                    transform.SetParent(slot.transform);
                    transform.position = slot.transform.position;
                    OccupySlots(slot.x, slot.y);

                    if (vestSlot)
                    {
                        itemRect.sizeDelta = new Vector2(itemWidth * 100f, itemHeight * 100f);
                        rt.pivot = new Vector2(pivotX, pivotY);
                        vestSlot = false;
                        playerEquipment?.SetEquipment(noneVest);
                    }
                    else if (enemyVestSlot)
                    {
                        rt.pivot = new Vector2(pivotX, pivotY);
                        enemyVestSlot = false;
                    }

                    slot.isTaken = true;
                    InventoryManager.Instance.UnregisterStashItem(itemUuId);

                    var data = new InventoryItemSaveData
                    {
                        itemId = itemId,
                        x = slot.x,
                        y = slot.y,
                        isRotated = isRotated,
                        itemWidth = this.itemWidth,
                        itemHeight = this.itemHeight,
                        equipmentType = this.equipmentType,
                        itemPrice = this.itemPrice,
                        itemUuId = this.itemUuId
                    };
                    InventoryManager.Instance.RegisterItem(data);
                }
                else
                {
                    ReturnItem();
                }
            }
            // Drop into Stash
            else if (target.CompareTag("StashSlot"))
            {
                Slot slot = target.GetComponent<Slot>();
                if (slot == null)
                {
                    ReturnItem();
                    canvasGroup.blocksRaycasts = true;
                    return;
                }

                InventoryPanel newPanel = slot.GetComponentInParent<InventoryPanel>();
                if (newPanel != null)
                    inventoryPanel = newPanel;

                CanPlaceItem(slot.x, slot.y);

                // Move from inventory to stash
                if (!shopSlot && canPlace)
                {
                    transform.SetParent(slot.transform);
                    transform.position = slot.transform.position;
                    OccupySlots(slot.x, slot.y);
                    slot.isTaken = true;
                    stashSlot = true;

                    InventoryManager.Instance.UnregisterItem(itemUuId);
                    var data = new InventoryItemSaveData
                    {
                        itemId = itemId,
                        x = slot.x,
                        y = slot.y,
                        isRotated = isRotated,
                        itemWidth = this.itemWidth,
                        itemHeight = this.itemHeight,
                        equipmentType = this.equipmentType,
                        itemPrice = this.itemPrice,
                        itemUuId = this.itemUuId
                    };
                    InventoryManager.Instance.RegisterStashItem(data);
                }
                // Purchase from shop into stash
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
                                shopSlot = false;
                                stashSlot = true;

                                if (string.IsNullOrEmpty(this.itemUuId))
                                    this.itemUuId = Guid.NewGuid().ToString();

                                var data = new InventoryItemSaveData
                                {
                                    itemId = itemId,
                                    x = slot.x,
                                    y = slot.y,
                                    isRotated = isRotated,
                                    itemWidth = this.itemWidth,
                                    itemHeight = this.itemHeight,
                                    equipmentType = this.equipmentType,
                                    itemPrice = this.itemPrice,
                                    itemUuId = this.itemUuId
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
                // Fallback stash logic (mirrors WeaponUI)
                else if (!shopSlot && canPlace)
                {
                    transform.SetParent(slot.transform);
                    transform.position = slot.transform.position;
                    OccupySlots(slot.x, slot.y);

                    if (vestSlot)
                    {
                        rt.pivot = new Vector2(pivotX, pivotY);
                        vestSlot = false;
                        playerEquipment?.SetEquipment(noneVest);
                    }
                    else if (enemyVestSlot)
                    {
                        rt.pivot = new Vector2(pivotX, pivotY);
                        enemyVestSlot = false;
                    }

                    slot.isTaken = true;
                    InventoryManager.Instance.UnregisterItem(itemUuId);

                    var data = new InventoryItemSaveData
                    {
                        itemId = itemId,
                        x = slot.x,
                        y = slot.y,
                        isRotated = isRotated,
                        itemWidth = this.itemWidth,
                        itemHeight = this.itemHeight,
                        equipmentType = this.equipmentType,
                        itemUuId = this.itemUuId
                    };
                    InventoryManager.Instance.RegisterStashItem(data);
                }
                else
                {
                    ReturnItem();
                }
            }
            // Sell from stash back to shop
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
                            InventoryManager.Instance.UnregisterItem(itemUuId);
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
            // Equip to character
            else if (target.CompareTag("VestSlot"))
            {
                vestSlot = true;
                EquipItem(target, itemRect);

                if (playerEquipment != null)
                    playerEquipment.SetEquipment(equipmentType);
                else
                    Debug.Log("메인메뉴라 VestSlot 없음");

                GameData.Instance.selectedVestType = equipmentType;

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
            // Delete slot
            else if (target.CompareTag("DeleteSlot"))
            {
                if (vestSlot)
                {
                    playerEquipment.SetEquipment(noneVest);
                    GameData.Instance.selectedVestType = PlayerEquipment.EquipmentType.NONEVEST;
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

        RectTransform slotRect = target.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 0.5f);
        itemRect.sizeDelta = slotRect.sizeDelta;
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

        if (vestSlot || enemyVestSlot)
            rt.pivot = new Vector2(0.5f, 0.5f);
    }

    private void CanPlaceItem(int startX, int startY)
    {
        canPlace = true;
        int width = isRotated ? itemHeight : itemWidth;
        int height = isRotated ? itemWidth : itemHeight;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int targetX = startX - x;
                int targetY = startY - y;
                if (targetX < 0 || targetY < 0 ||
                    targetX >= inventoryPanel.columns || targetY >= inventoryPanel.rows ||
                    inventoryPanel.slots[targetX, targetY].isTaken)
                {
                    canPlace = false;
                    return;
                }
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

    public void Init(InventoryPanel inventory, PlayerEquipment equipment, bool EnemyVestSlot = false)
    {
        inventoryPanel = inventory;
        playerEquipment = equipment;
        enemyVestSlot = EnemyVestSlot;
    }
}
