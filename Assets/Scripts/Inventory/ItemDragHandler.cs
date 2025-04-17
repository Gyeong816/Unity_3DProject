using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Transform originalParent;
    private Vector2 originalPosition;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private bool isDragging = false; 
    private bool isrotated = false;
    public int itemWidth = 3;
    public int itemHeight = 2;
    private bool originalRotation = false;

    
    private Image itemImage;


    public float pivotX ;
    public float pivotY;
    public float rotatedpivotY ;
    
    private List<Slot> occupiedSlots = new List<Slot>();
    public InventoryPanel inventoryPanel;
    private RectTransform rt;
    
    private bool canPlace = false;

    private bool takeWeaponSlot = false;
     
    
    private float originalWidth;
    private float originalHeight;


    public PlayerWeapon PlayerWeapon;
    
    
    void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        itemImage = GetComponent<Image>();
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
      
        if (isDragging && Input.GetKeyDown(KeyCode.R) )
        {
            
            if (!isrotated)
            {
                isrotated = false;

                
                rt.pivot = new Vector2(pivotX, rotatedpivotY);
                

                
                
                isrotated = true;

                itemImage.rectTransform.localEulerAngles = new Vector3(0, 0, -90f);
            }
            else
            {
                isrotated = false;
                
              
                
                rt.pivot = new Vector2(pivotX, pivotY);
                

                itemImage.rectTransform.localEulerAngles = new Vector3(0, 0, 0f);
            }
            
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
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    
    
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        
        
        FreeOccupiedSlots();
        
        
        GameObject target = eventData.pointerEnter;
        
        RectTransform itemRect = GetComponent<RectTransform>();


        
        if (target.CompareTag("Slot"))
        {
            
            Slot slot = target.GetComponent<Slot>();

            CanPlaceItem(slot.x, slot.y);
            
            if (canPlace)
            {
                transform.SetParent(target.transform);
                transform.position = target.transform.position;

                OccupySlots(slot.x, slot.y);

                if (takeWeaponSlot)
                {
                    itemRect.sizeDelta = new Vector2(200f, 100f);
                    rt.pivot = new Vector2(pivotX, pivotY);
                    takeWeaponSlot = false;
                }
                
            }
            else
            {
                ReturnItem();  
            }
            
            
        }
        else if (target.CompareTag("WeaponSlot"))
        {
            takeWeaponSlot = true;
            transform.SetParent(target.transform);
            transform.position = target.transform.position;
            
            RectTransform slotRect = target.GetComponent<RectTransform>();
            
            rt.pivot = new Vector2(0.5f, 0.5f);
            
            itemRect.sizeDelta = slotRect.sizeDelta;


            PlayerWeapon.SetWeapon(PlayerWeapon.WeaponType.AK47);

        }
        else
        {
            ReturnItem();

        }

        canvasGroup.blocksRaycasts = true;

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
       else if (takeWeaponSlot)
       {
           rt.pivot = new Vector2(0.5f, 0.5f);
           itemImage.rectTransform.localEulerAngles = Vector3.zero;
       }
       else
       {
           rt.pivot = new Vector2(pivotX, pivotY);
           itemImage.rectTransform.localEulerAngles = Vector3.zero;

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
                int targetX =  startX - x;
                int targetY =  startY - y;

                if (targetX < 0 || targetY < 0 ||
                    targetX >= inventoryPanel.columns || targetY >= inventoryPanel.rows)
                {
                    canPlace = false;
                    return;
                }

                if (inventoryPanel.slots[targetX, targetY].isTaken)
                {
                    Debug.Log("배치 불가, 슬롯 점유 중");
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
                int targetX =  startX - x;
                int targetY =  startY - y;


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