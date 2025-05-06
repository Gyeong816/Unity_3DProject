using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
   [Header("Item Info")]
    public int itemWidth = 1;
    public int itemHeight = 1;
    public bool isWeapon = false;
    public bool isHelmet = false;
    public bool isVest = false;
    public bool isBullet = false;

    public string itemId;
    
    
    public PlayerWeapon.WeaponType weaponType;
    private PlayerWeapon.WeaponType noneWeapon = PlayerWeapon.WeaponType.NONE;
    
    public PlayerWeapon.BulletType bulletType;
   
    
    public PlayerEquipment.EquipmentType equipmentType;
    
    private PlayerEquipment.EquipmentType noneHelmet = PlayerEquipment.EquipmentType.NONEHELMET;
    private PlayerEquipment.EquipmentType noneVest = PlayerEquipment.EquipmentType.NONEVEST;

    [Header("Pivot Settings")]
    public float pivotX = 0.5f;
    public float pivotY = 0.5f;
    public float rotatedpivotY = 0.5f;

    [Header("References")]
    public InventoryPanel inventoryPanel;
    public PlayerWeapon playerWeapon;
    public PlayerEquipment playerEquipment;

    [Header("Bullet Info")]
    public int bulletAmount = 30; 
    
    private Transform originalParent;
    private Vector2 originalPosition;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rt;
    private Image itemImage;

    private bool isDragging = false; 
    private bool isrotated = false;
    private bool originalRotation = false;

    private bool weaponSlot = false;
    private bool helmetSlot = false;
    private bool vestSlot = false;

    
    private bool canPlace = false;



    private List<Slot> occupiedSlots = new List<Slot>();

    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        itemImage = GetComponentInChildren<Image>();
        rt = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (!isDragging) 
        {
            Slot slot = GetComponentInParent<Slot>();
            if (slot != null)
            {
                OccupySlots(slot.x, slot.y);
            }
        }
    }

    void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            isrotated = !isrotated;
            rt.pivot = new Vector2(pivotX, isrotated ? rotatedpivotY : pivotY);
            itemImage.rectTransform.localEulerAngles = isrotated ? new Vector3(0, 0, -90f) : Vector3.zero;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDragging = true;
        originalParent = transform.parent;
        originalPosition = transform.position;
        originalRotation = isrotated;

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
        isDragging = false;
        GameObject target = eventData.pointerEnter;
        RectTransform itemRect = GetComponent<RectTransform>();

        if (target != null && target.CompareTag("Slot"))
        {
            Slot slot = target.GetComponent<Slot>();
            CanPlaceItem(slot.x, slot.y);

            if (canPlace)
            {
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                OccupySlots(slot.x, slot.y);

                if (weaponSlot)
                {
                    itemRect.sizeDelta = new Vector2(itemWidth * 100f, itemHeight * 100f);
                    rt.pivot = new Vector2(pivotX, pivotY);
                    weaponSlot = false;
                    playerWeapon.SetWeapon(noneWeapon);
                }
                else if (helmetSlot)
                {
                    itemRect.sizeDelta = new Vector2(itemWidth * 100f, itemHeight * 100f);
                    rt.pivot = new Vector2(pivotX, pivotY);
                    helmetSlot = false;
                    playerEquipment.SetEquipment(noneHelmet);
                }
                else if (vestSlot) 
                {
                    itemRect.sizeDelta = new Vector2(itemWidth * 100f, itemHeight * 100f);
                    rt.pivot = new Vector2(pivotX, pivotY);
                    vestSlot = false;
                    playerEquipment.SetEquipment(noneVest);

                }
            }
            else
            {
                ReturnItem();  
            }
        }
        else if (target.CompareTag("WeaponSlot") && isWeapon)
        {
            weaponSlot = true;

            EquipItem(target, itemRect);

            playerWeapon.SetWeapon(weaponType);
        }
        else if (target.CompareTag("HelmetSlot") && isHelmet)
        {
            helmetSlot = true;

            EquipItem(target, itemRect);

            playerEquipment.SetEquipment(equipmentType);
        }
        else if (target.CompareTag("VestSlot") && isVest)
        {
            vestSlot = true;

            EquipItem(target, itemRect);

            playerEquipment.SetEquipment(equipmentType);
        }
        else if (target.CompareTag("BulletSlot") && isBullet)
        {
            var currentWeaponType = playerWeapon.GetCurrentWeaponType();

            
            if (currentWeaponType == PlayerWeapon.WeaponType.NONE)
            {
                Debug.Log("무기가 장착되지 않았습니다. 총알 장착 불가.");
                ReturnItem();
                return;
            }

           
            if ((currentWeaponType == PlayerWeapon.WeaponType.AK47 && bulletType != PlayerWeapon.BulletType.AK47Bullet) ||
                (currentWeaponType == PlayerWeapon.WeaponType.M3 && bulletType != PlayerWeapon.BulletType.M3Bullet))
            {
                Debug.Log("무기와 총알 타입이 맞지 않습니다.");
                ReturnItem();
                return;
            }

            Debug.Log($"총알 {bulletType} 장착됨 - 수량: {bulletAmount}");

            EquipItem(target, itemRect);

            playerWeapon.SetBullet(bulletType);
            playerWeapon.AddReserveAmmo(bulletAmount);
            
        }
        else
        {
            ReturnItem();
        }

        canvasGroup.blocksRaycasts = true;
    }


    public InventoryItemSaveData GetSaveData()
    {
        Slot slot = GetComponentInParent<Slot>();
        if (slot == null) return null;

        return new InventoryItemSaveData
        {
            itemId = this.itemId,
            x = slot.x,
            y = slot.y,
            isRotated = this.isrotated
        };
    }
    
    
    
    
    
    

    public void Init(InventoryPanel inventory, PlayerWeapon weapon, PlayerEquipment equipment, bool EnemyVestSlot = false)
    {
        inventoryPanel = inventory;
        playerWeapon = weapon;
        playerEquipment = equipment;

        vestSlot = EnemyVestSlot;
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

        isrotated = originalRotation;

        if (isrotated)
        {
            rt.pivot = new Vector2(pivotX, rotatedpivotY);
            itemImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90f);
        }
        else
        {
            rt.pivot = new Vector2(pivotX, pivotY);
            itemImage.rectTransform.localEulerAngles = Vector3.zero;
        }

        if(weaponSlot || vestSlot || helmetSlot )
        {
            rt.pivot = new Vector2(0.5f, 0.5f);
        }
    }

    private void CanPlaceItem(int startX, int startY)
    {
        canPlace = true;

        int width = isrotated ? itemHeight : itemWidth;
        int height = isrotated ? itemWidth : itemHeight;

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

    private void OccupySlots(int startX, int startY)
    {
        occupiedSlots.Clear();

        int width = isrotated ? itemHeight : itemWidth;
        int height = isrotated ? itemWidth : itemHeight;

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
}