using System;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class BulletUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Bullet Info")]
    public string itemId;
    public string itemUuId;
    public PlayerWeapon.BulletType bulletType;
    private PlayerWeapon.BulletType noneBullet = PlayerWeapon.BulletType.NONE;
    public int bulletAmount = 30;
    public int itemPrice = 50;

    [Header("References")]
    public InventoryPanel inventoryPanel;
    public PlayerWeapon playerWeapon;
    public TextMeshProUGUI ammoText;

    // shop/stash flags
    public bool shopSlot = false;
    public bool stashSlot = false;

    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private RectTransform rt;

    public Transform originalParent;
    public Vector2 originalPosition;
    public Slot currentSlot;

    private bool bulletSlot = false;

    void Start()
    {
        // 레퍼런스 초기화
        canvas = canvas ?? GetComponentInParent<Canvas>();
        canvasGroup = canvasGroup ?? GetComponent<CanvasGroup>();
        rt = rt ?? GetComponent<RectTransform>();

        // 로드된 상태라면, 현재 슬롯을 찾고 위치 기억
        currentSlot = GetComponentInParent<Slot>();
        if (currentSlot != null)
        {
            currentSlot.isTaken = true;
            originalParent = currentSlot.transform;
            originalPosition = currentSlot.transform.position;
        }

        // UUID 보장
        if (string.IsNullOrEmpty(itemUuId))
            itemUuId = Guid.NewGuid().ToString();

        // 텍스트 갱신
        if (ammoText != null)
            ammoText.text = bulletAmount.ToString();
    }

    void Update()
    {
        // 탄창이 비면 제거
        if (bulletAmount <= 0)
        {
            Destroy(gameObject);
            return;
        }

        // UI 텍스트 갱신
        if (ammoText != null)
            ammoText.text = bulletAmount.ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 시 원위치 저장
        originalParent = transform.parent;
        originalPosition = transform.position;

        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;

        // 이전 슬롯 비우기
        if (currentSlot != null)
            currentSlot.isTaken = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject target = eventData.pointerEnter;

        // 1) 장착 슬롯에 끼우기
        if (target != null && target.CompareTag("BulletSlot"))
        {
            InventoryManager.Instance.UnregisterItem(itemUuId);
            TryEquipBullet(target);
        }
        // 2) 인벤토리 슬롯에 놓기
        else if (target != null && target.CompareTag("PlayerSlot"))
        {
            Slot slot = target.GetComponent<Slot>();
            if (slot != null && !slot.isTaken)
            {
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                slot.isTaken = true;

                // 위치 기억
                currentSlot = slot;
                originalParent = slot.transform;
                originalPosition = slot.transform.position;

                // 장착화 해제
                if (bulletSlot)
                {
                    playerWeapon?.SetBullet(noneBullet);
                    bulletSlot = false;
                }
                InventoryManager.Instance.UnregisterStashItem(itemUuId);
                // 저장
                var data = new InventoryItemSaveData
                {
                    itemId       = itemId,
                    itemUuId     = itemUuId,
                    x            = slot.x,
                    y            = slot.y,
                    bulletAmount = bulletAmount,
                    bulletType = bulletType,
                    itemPrice    = itemPrice
                };
                InventoryManager.Instance.RegisterItem(data);

                // 플래그 초기화
                stashSlot = false;
                shopSlot  = false;
            }
            else ReturnItem();
        }
        // 3) stash 슬롯에 놓기 (구매 또는 이동)
        else if (target != null && target.CompareTag("StashSlot"))
        {
            Slot slot = target.GetComponent<Slot>();
            if (slot == null || slot.isTaken)
            {
                ReturnItem();
                canvasGroup.blocksRaycasts = true;
                return;
            }

            // 구매 로직
            if (shopSlot)
            {
                TradeManager.Instance.ShowConfirm(false, itemId, itemPrice,
                    () =>
                    {
                        if (TradeManager.Instance.CanPlayerAfford(itemPrice))
                        {
                            TradeManager.Instance.SubtractPlayerDoller(itemPrice);
                            TradeManager.Instance.AddVendorDoller(itemPrice);

                            // stash에 배치
                            transform.SetParent(slot.transform);
                            transform.position = slot.transform.position;
                            slot.isTaken = true;
                            stashSlot = true;
                            shopSlot  = false;

                            originalParent   = slot.transform;
                            originalPosition = slot.transform.position;

                            if (string.IsNullOrEmpty(itemUuId))
                                itemUuId = Guid.NewGuid().ToString();

                            var data = new InventoryItemSaveData
                            {
                                itemId       = itemId,
                                itemUuId     = itemUuId,
                                x            = slot.x,
                                y            = slot.y,
                                bulletAmount = bulletAmount,
                                bulletType = bulletType,
                                itemPrice    = itemPrice
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
            // 일반 stash 이동
            else
            {
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                slot.isTaken = true;
                stashSlot = true;

                originalParent   = slot.transform;
                originalPosition = slot.transform.position;

                InventoryManager.Instance.UnregisterItem(itemUuId);
                var data = new InventoryItemSaveData
                {
                    itemId       = itemId,
                    itemUuId     = itemUuId,
                    x            = slot.x,
                    y            = slot.y,
                    bulletAmount = bulletAmount,
                    bulletType = bulletType,
                    itemPrice    = itemPrice
                };
                InventoryManager.Instance.RegisterStashItem(data);
            }
        }
        // 4) 판매
        else if (target != null && target.CompareTag("ShopSlot") && stashSlot)
        {
            TradeManager.Instance.ShowConfirm(true, itemId, itemPrice,
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
        else if (target.CompareTag("DeleteSlot"))
        {

            InventoryManager.Instance.UnregisterItem(itemUuId);
            InventoryManager.Instance.UnregisterStashItem(itemUuId);
            Destroy(gameObject);
        }
        else
        {
            ReturnItem();
        }

        canvasGroup.blocksRaycasts = true;
    }

    private void TryEquipBullet(GameObject target)
    {
        if (playerWeapon == null)
        {
            Debug.LogWarning("[BulletUI] PlayerWeapon이 연결되어 있지 않습니다.");
            ReturnItem();
            return;
        }

        var currentWeaponType = playerWeapon.GetCurrentWeaponType();
        if (currentWeaponType == PlayerWeapon.WeaponType.NONE)
        {
            Debug.Log("현재 무기 없음 - 총알 장착 불가");
            ReturnItem();
            return;
        }

        if ((currentWeaponType == PlayerWeapon.WeaponType.AK47 && bulletType != PlayerWeapon.BulletType.AK47Bullet) ||
            (currentWeaponType == PlayerWeapon.WeaponType.M3 && bulletType != PlayerWeapon.BulletType.M3Bullet))
        {
            Debug.Log("무기 타입과 총알 타입이 일치하지 않습니다.");
            ReturnItem();
            return;
        }

        transform.SetParent(target.transform);
        transform.position = target.transform.position;
        playerWeapon.SetBullet(bulletType);
        playerWeapon.AddReserveAmmo(bulletAmount);
        playerWeapon.SetBullet(bulletType, this);
        bulletSlot = true;

        // 장착 슬롯이므로 위치 기억
        currentSlot = target.GetComponent<Slot>();
        if (currentSlot != null)
        {
            originalParent   = currentSlot.transform;
            originalPosition = currentSlot.transform.position;
        }
    }

    private void ReturnItem()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        if (currentSlot != null)
            currentSlot.isTaken = true;
    }



    public void ReduceAmmo(int amount)
    {
        bulletAmount -= amount;
        if (bulletAmount <= 0)
        {
            playerWeapon.SetBullet(PlayerWeapon.BulletType.NONE);
            Destroy(gameObject);
        }

    }
}
