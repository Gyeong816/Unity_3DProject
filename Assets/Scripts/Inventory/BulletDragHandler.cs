using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class BulletDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Bullet Info")]
    public PlayerWeapon.BulletType bulletType;
    public int bulletAmount = 30;

    [Header("Ammo UI")]
    public TextMeshProUGUI ammoText;
    
    [Header("References")]
    public PlayerWeapon playerWeapon;

    private Transform originalParent;
    private Vector2 originalPosition;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rt;

    private Slot currentSlot;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        rt = GetComponent<RectTransform>();
        canvasGroup.blocksRaycasts = true; 
    }

    void Start()
    {
        currentSlot = GetComponentInParent<Slot>();
        if (currentSlot != null)
        {
            currentSlot.isTaken = true;
        }
    }

    void Update()
    {
        if (bulletAmount <= 0)
        {
            Destroy(gameObject);
        }
        
        ammoText.text = $"{bulletAmount}";
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

        if (target != null && target.CompareTag("BulletSlot"))
        {
  

            var currentWeaponType = playerWeapon.GetCurrentWeaponType();
            
            if (currentWeaponType == PlayerWeapon.WeaponType.NONE)
            {
                Debug.Log("무기 없음 - 총알 장착 불가");
                ReturnItem();
                return;
            }

            if ((currentWeaponType == PlayerWeapon.WeaponType.AK47 && bulletType != PlayerWeapon.BulletType.AK47Bullet) ||
                (currentWeaponType == PlayerWeapon.WeaponType.M3 && bulletType != PlayerWeapon.BulletType.M3Bullet))
            {
                Debug.Log("무기와 총알 타입 불일치");
                ReturnItem();
                return;
            }

            transform.SetParent(target.transform);
            transform.position = target.transform.position;

            Slot newSlot = target.GetComponent<Slot>();
            if (newSlot != null)
            {
                newSlot.isTaken = true;
                currentSlot = newSlot;
            }

            playerWeapon.SetBullet(bulletType);
            playerWeapon.AddReserveAmmo(bulletAmount);
          //  playerWeapon.SetBullet(bulletType, this);
            
        }
        else if (target != null && target.CompareTag("Slot"))
        {
           

            Slot slot = target.GetComponent<Slot>();

            if (CanPlaceItem(slot))
            {
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                slot.isTaken = true;
                currentSlot = slot;
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

    private void ReturnItem()
    {
        

        transform.SetParent(originalParent);
        transform.position = originalPosition;

        currentSlot = originalParent.GetComponent<Slot>();
        if (currentSlot != null)
        {
            currentSlot.isTaken = true;
        }

        canvasGroup.blocksRaycasts = true; 
    }
    private void FreeOccupiedSlots()
    {
        if (currentSlot != null)
        {
            currentSlot.isTaken = false;
        }
    }

    private bool CanPlaceItem(Slot slot)
    {
        return !slot.isTaken;
    }
    
    public void ReduceAmmo(int amount)
    {
        bulletAmount -= amount;
        if (bulletAmount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
