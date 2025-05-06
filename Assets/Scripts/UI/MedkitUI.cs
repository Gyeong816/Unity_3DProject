using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MedkitUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string itemId;
    public string itemUuId;
    public int healAmount = 30;
    public int itemPrice = 100;

    public PlayerHP playerHP;

    public bool shopSlot = false;
    public bool stashSlot = false;

    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public InventoryPanel inventoryPanel;
    public Transform originalParent;
    public Vector2 originalPosition;
    public Slot currentSlot;

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();

        currentSlot = GetComponentInParent<Slot>();
        if (currentSlot != null)
        {
            currentSlot.isTaken = true;
            originalParent = currentSlot.transform;
            originalPosition = currentSlot.transform.position;
        }

        if (string.IsNullOrEmpty(itemUuId))
            itemUuId = Guid.NewGuid().ToString();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        originalPosition = transform.position;

        transform.SetParent(canvas.transform);
        canvasGroup.blocksRaycasts = false;

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

        if (target != null && target.CompareTag("HPSlot"))
        {
            ApplyHealing();

            InventoryManager.Instance.UnregisterItem(itemUuId);
            InventoryManager.Instance.UnregisterStashItem(itemUuId);

            Destroy(gameObject);
        }
        else if (target.CompareTag("DeleteSlot"))
        {

            InventoryManager.Instance.UnregisterItem(itemUuId);
            InventoryManager.Instance.UnregisterStashItem(itemUuId);
            Destroy(gameObject);
        }
        else if (target != null && target.CompareTag("PlayerSlot"))
        {
            Slot slot = target.GetComponent<Slot>();
            if (slot != null && !slot.isTaken)
            {
                transform.SetParent(slot.transform);
                transform.position = slot.transform.position;
                slot.isTaken = true;
                currentSlot = slot;

                originalParent = slot.transform;
                originalPosition = slot.transform.position;

                if (stashSlot)
                    InventoryManager.Instance.UnregisterStashItem(itemUuId);
                else
                    InventoryManager.Instance.UnregisterItem(itemUuId);

                var data = new InventoryItemSaveData
                {
                    itemId = itemId,
                    itemUuId = itemUuId,
                    x = slot.x,
                    y = slot.y,
                    healAmount = healAmount,
                    itemPrice = itemPrice
                };
                InventoryManager.Instance.RegisterItem(data);

                stashSlot = false;
                shopSlot = false;

                canvasGroup.blocksRaycasts = true;
                return;
            }
        }
        else if (target != null && target.CompareTag("StashSlot"))
        {
            Slot slot = target.GetComponent<Slot>();
            if (slot == null || slot.isTaken)
            {
                ReturnItem();
                canvasGroup.blocksRaycasts = true;
                return;
            }

            if (shopSlot)
            {
                TradeManager.Instance.ShowConfirm(false, itemId, itemPrice,
                    () =>
                    {
                        if (TradeManager.Instance.CanPlayerAfford(itemPrice))
                        {
                            TradeManager.Instance.SubtractPlayerDoller(itemPrice);
                            TradeManager.Instance.AddVendorDoller(itemPrice);

                            MoveToSlot(slot);
                            RegisterStash();
                        }
                        else
                        {
                            TradeManager.Instance.ShowWarningPanel();
                            ReturnItem();
                        }

                        canvasGroup.blocksRaycasts = true;
                    },
                    () =>
                    {
                        ReturnItem();
                        canvasGroup.blocksRaycasts = true;
                    }
                );
                return;
            }
            else
            {
                MoveToSlot(slot);
                InventoryManager.Instance.UnregisterItem(itemUuId);
                RegisterStash();
                canvasGroup.blocksRaycasts = true;
                return;
            }
        }
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

                    canvasGroup.blocksRaycasts = true;
                },
                () =>
                {
                    ReturnItem();
                    canvasGroup.blocksRaycasts = true;
                }
            );
            return;
        }
        else
        {
            ReturnItem();
        }
         canvasGroup.blocksRaycasts = true;
    }

    private void ApplyHealing()
    {
        if (playerHP == null)
        {
            Debug.LogWarning("[MedkitUI] PlayerHP가 연결되어 있지 않습니다.");
            ReturnItem();
            canvasGroup.blocksRaycasts = true;
            return;
        }

        playerHP.Heal(healAmount);
        InventoryManager.Instance.UnregisterItem(itemUuId);
        InventoryManager.Instance.UnregisterStashItem(itemUuId);
        Destroy(gameObject);
        canvasGroup.blocksRaycasts = true;
    }

    private void MoveToSlot(Slot slot)
    {
        transform.SetParent(slot.transform);
        transform.position = slot.transform.position;
        slot.isTaken = true;
        stashSlot = true;
        shopSlot = false;

        currentSlot = slot;
        originalParent = slot.transform;
        originalPosition = slot.transform.position;
    }

    private void RegisterStash()
    {
        if (string.IsNullOrEmpty(itemUuId))
            itemUuId = Guid.NewGuid().ToString();

        var data = new InventoryItemSaveData
        {
            itemId = itemId,
            itemUuId = itemUuId,
            x = currentSlot.x,
            y = currentSlot.y,
            healAmount = healAmount,
            itemPrice = itemPrice
        };
        InventoryManager.Instance.RegisterStashItem(data);
    }

    private void ReturnItem()
    {
        transform.SetParent(originalParent);
        transform.position = originalPosition;
        if (currentSlot != null)
            currentSlot.isTaken = true;
    }
}
