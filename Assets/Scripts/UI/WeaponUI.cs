using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponUI: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   [Header("Item Info")]
    public int itemWidth = 4;
    public int itemHeight = 2;
    public string itemUuId;
    public string itemId;
    public int itemPrice = 100;
    
    
    public PlayerWeapon.WeaponType weaponType;
    private PlayerWeapon.WeaponType noneWeapon = PlayerWeapon.WeaponType.NONE;
    
  

    [Header("Pivot Settings")]
    public float pivotX = 0.88f;
    public float pivotY = 0.25f;
    public float rotatedpivotY = 0.75f;

    [Header("References")]
    public InventoryPanel inventoryPanel;
    public PlayerWeapon playerWeapon;


    
    
    private Transform originalParent;
    private Vector2 originalPosition;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rt;
    public Image itemImage;
    private Image backgroundImage;
    
    
    private bool isDragging; 
    public bool isRotated;
    private bool originalRotation;

    public bool weaponSlot;
    private bool enemyWeaponSlot;
    private bool canPlace;

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
            rt.pivot = new Vector2(pivotX, isRotated ? rotatedpivotY : pivotY);
            backgroundImage.rectTransform.localEulerAngles = isRotated ? new Vector3(0, 0, -90f) : Vector3.zero;
        }
    }
    public void ApplyRotation()
    {
        rt.pivot = new Vector2(pivotX, rotatedpivotY);
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
        
        if (weaponSlot || enemyWeaponSlot)
        {
           
            rt.pivot = new Vector2(pivotX, pivotY);
            
            int width = isRotated ? itemHeight : itemWidth;
            int height = isRotated ? itemWidth : itemHeight;
            rt.sizeDelta = new Vector2(width * 100f, height * 100f);
            itemImage.rectTransform.sizeDelta = new Vector2(380f, 180f);
            
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
                {
                    inventoryPanel = newPanel;
                }

                CanPlaceItem(slot.x, slot.y);

                if (canPlace)
                {
                    transform.SetParent(slot.transform);
                    transform.position = slot.transform.position;
                    OccupySlots(slot.x, slot.y);

                    if (weaponSlot)
                    {
                        rt.pivot = new Vector2(pivotX, pivotY);
                        weaponSlot = false;
                        playerWeapon?.SetWeapon(noneWeapon);
                    }
                    else if (enemyWeaponSlot)
                    {
                        rt.pivot = new Vector2(pivotX, pivotY);
                        enemyWeaponSlot = false;
                    }
                    
                    slot.isTaken = true;
                    
                    InventoryManager.Instance.UnregisterStashItem(itemUuId);

                    InventoryItemSaveData data = new InventoryItemSaveData
                    {
                        itemId = itemId,
                        x = slot.x,
                        y = slot.y,
                        isRotated = this.isRotated,
                        itemWidth = this.itemWidth,
                        itemHeight = this.itemHeight,
                        weaponType = this.weaponType,
                        itemPrice = this.itemPrice,
                        itemUuId = this.itemUuId,
                    };
                    InventoryManager.Instance.RegisterItem(data);
                
                }
                else
                {
                    ReturnItem();
                }
            }
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
                {
                    inventoryPanel = newPanel;
                }
                

                CanPlaceItem(slot.x, slot.y);
                
                if (!shopSlot&&canPlace)
                {
                    transform.SetParent(slot.transform);
                    transform.position = slot.transform.position;
                    OccupySlots(slot.x, slot.y);

                    slot.isTaken = true;
                    stashSlot = true;
                    
               
                    InventoryManager.Instance.UnregisterItem(itemUuId);
                    
                    InventoryItemSaveData data = new InventoryItemSaveData
                    {
                        itemId = itemId,
                        x = slot.x,
                        y = slot.y,
                        isRotated = this.isRotated,
                        itemWidth = this.itemWidth,
                        itemHeight = this.itemHeight,
                        weaponType = this.weaponType,
                        itemPrice = this.itemPrice,
                        itemUuId = this.itemUuId,
                    };
                    InventoryManager.Instance.RegisterStashItem(data);
                }
                else if (shopSlot&&canPlace)
                {
                    TradeManager.Instance.ShowConfirm(false,itemId, itemPrice, 
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
                                {
                                    this.itemUuId = Guid.NewGuid().ToString();
                                }

                                InventoryItemSaveData data = new InventoryItemSaveData
                                {
                                    itemId = itemId,
                                    x = slot.x,
                                    y = slot.y,
                                    isRotated = this.isRotated,
                                    itemWidth = this.itemWidth,
                                    itemHeight = this.itemHeight,
                                    weaponType = this.weaponType,
                                    itemPrice = this.itemPrice,
                                    itemUuId = this.itemUuId,
                                };
                                InventoryManager.Instance.RegisterStashItem(data);
                                
                            }
                            else
                            {
                                TradeManager.Instance.ShowWarningPanel();
                                ReturnItem();
                            }
                            
                        },
                        () =>
                        {
                            ReturnItem();
                        });
                    
                }
                else if (!shopSlot && canPlace)
                {
                    transform.SetParent(slot.transform);
                    transform.position = slot.transform.position;
                    OccupySlots(slot.x, slot.y);

                    if (weaponSlot)
                    {
                        rt.pivot = new Vector2(pivotX, pivotY);
                        weaponSlot = false;
                        playerWeapon?.SetWeapon(noneWeapon);
                    }
                    else if (enemyWeaponSlot)
                    {
                        rt.pivot = new Vector2(pivotX, pivotY);
                        enemyWeaponSlot = false;
                    }
                    
                    slot.isTaken = true;
                    
                    InventoryManager.Instance.UnregisterItem(itemUuId);

                    InventoryItemSaveData data = new InventoryItemSaveData
                    {
                        itemId = itemId,
                        x = slot.x,
                        y = slot.y,
                        isRotated = isRotated,
                        itemWidth = itemWidth,
                        itemHeight = itemHeight,
                        weaponType = weaponType,
                        itemUuId = this.itemUuId,
                    };
                    InventoryManager.Instance.RegisterStashItem(data);
                    
                    
                    
                }
                else
                {
                    ReturnItem();
                }
            }
            else if (target.CompareTag("ShopSlot")&&stashSlot)
            {
                TradeManager.Instance.ShowConfirm(true, itemId, itemPrice,
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
                    () =>
                    {
                        ReturnItem();
                    });
                
            }
            else if (target.CompareTag("WeaponSlot"))
            {
                weaponSlot = true;
                EquipItem(target, itemRect);

                if (playerWeapon != null)
                {
                    playerWeapon.SetWeapon(weaponType);
                }
                else
                {
                    Debug.Log("메인메뉴라 playerWeapon 없음");
                }

                GameData.Instance.selectedWeaponType = weaponType;
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
            else if (target.CompareTag("DeleteSlot"))
            {
                if (weaponSlot)
                {
                    playerWeapon.SetWeapon(noneWeapon);
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
        {
            slot.isTaken = true;
        }

        isRotated = originalRotation;

        if (isRotated)
        {
            rt.pivot = new Vector2(pivotX, rotatedpivotY);
            backgroundImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90f);
        }
        else
        {
            rt.pivot = new Vector2(pivotX, pivotY);
            backgroundImage.rectTransform.localEulerAngles = Vector3.zero;
        }

        if(weaponSlot || enemyWeaponSlot)
        {
            rt.pivot = new Vector2(0.5f, 0.5f);
        }

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
                    targetX >= inventoryPanel.columns || targetY >= inventoryPanel.rows)
                {
                    canPlace = false;
                    return;
                }

                if (inventoryPanel.slots[targetX, targetY].isTaken)
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
        {
            for (int y = 0; y < height; y++)
            {
                int targetX = startX - x;
                int targetY = startY - y;

                Slot slot = inventoryPanel.slots[targetX, targetY];
                slot.isTaken = true;
                occupiedSlots.Add(slot);
            }
        }
    }

    private void FreeOccupiedSlots()
    {
        foreach (var slot in occupiedSlots)
        {
            slot.isTaken = false;
        }
        occupiedSlots.Clear();
    }
    
    public void Init(InventoryPanel inventory,  PlayerWeapon weapon, bool EnemyWeaponSlot = false)
    {
        inventoryPanel = inventory;

        playerWeapon = weapon;

        enemyWeaponSlot = EnemyWeaponSlot;
    }
}