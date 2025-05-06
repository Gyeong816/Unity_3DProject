using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TradeManager : MonoBehaviour
{
    public static TradeManager Instance;

    public GameObject confirmPanel;
    public GameObject warningPanel;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI priceText;
    public GameObject BuyButton;
    public GameObject SellButton;
    
    [Header("Money")]
    public int playerDollar= 150;
    public int vendorDollar = 10000;

    public TextMeshProUGUI playerDollarText;
    public TextMeshProUGUI vendorDollarText;
    

    private Action onConfirm;
    private Action onCancel;

    private bool isSelling;
    private void Awake()
    {
        
        
        Instance = this;
        confirmPanel.SetActive(false);
        warningPanel.SetActive(false);
        
    }

    private void Start()
    {
        playerDollar = GameData.Instance.playerDollar;
        vendorDollar = GameData.Instance.vendorDollar;
        
        UpdateDollarUI();
    }

    public void ShowWarningPanel()
    {
        warningPanel.SetActive(true);
    }
    public void HideWarningPanel()
    {
        warningPanel.SetActive(false);
    }
    
    public void ShowConfirm(bool isSelling, string itemName, int price, Action confirmCallback, Action cancelCallback)
    {
        itemNameText.text = itemName;
        priceText.text = $"${price}";

        onConfirm = confirmCallback;
        onCancel = cancelCallback;
        confirmPanel.SetActive(true);
        if (isSelling)
        {
            BuyButton.SetActive(false);
            SellButton.SetActive(true);
        }
        else
        {
            BuyButton.SetActive(true);
            SellButton.SetActive(false);
        }
    }
    
    public void OnConfirm()
    {
        confirmPanel.SetActive(false);
        onConfirm?.Invoke();
    }

    public void OnCancel()
    {
        confirmPanel.SetActive(false);
        onCancel?.Invoke();
    }
    
    
    public void AddPlayerDoller(int amount)
    {
        playerDollar += amount;
        GameData.Instance.playerDollar = playerDollar;
        UpdateDollarUI();
    }

    public void SubtractPlayerDoller(int amount)
    {
        playerDollar -= amount;
        GameData.Instance.playerDollar = playerDollar;
        UpdateDollarUI();
    }

    public void AddVendorDoller(int amount)
    {
        vendorDollar += amount;
        GameData.Instance.vendorDollar = vendorDollar;
        UpdateDollarUI();
    }

    public void SubtractVendorDoller(int amount)
    {
        vendorDollar -= amount;
        GameData.Instance.vendorDollar = vendorDollar;
        UpdateDollarUI();
    }

    private void UpdateDollarUI()
    {
        playerDollarText.text = $"Your money (${playerDollar})";
        vendorDollarText.text = $"Vendors money (${vendorDollar})";
    }
    
    
    
    
    public bool CanPlayerAfford(int amount)
    {
        return playerDollar >= amount;
    }
    public bool CanVendorAfford(int amount)
    {
        return vendorDollar >= amount;
    }
}

