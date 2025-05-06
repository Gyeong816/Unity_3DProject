using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Ammo UI")]
    public TextMeshProUGUI ammoText;

    public InGameInventoryLoader inGameInventoryLoader;
    public Crosshair crosshair;
    [Header("HP UI")]
    public TextMeshProUGUI hpText;

    [Header("Player Reference")]
    public PlayerWeapon playerWeapon;

    [Header("Canvas")]
    public Canvas ingameCanvas;
    public Canvas inventoryCanvas;
    public Canvas exitCanvas;
    public Canvas deathCanvas;

    [Header("Camera Control")]
    public CameraController cameraController;

    [Header("Loot Prompt UI")]
    public RectTransform lootPromptUI;
    public Vector3 worldOffset = new Vector3(0, 2f, 0);


    [Header("Enemy Inventories")]
    public GameObject enemyInventory;
    public EnemyInventory enemyInven;
    private EnemyController currentEnemy;

    [Header("LootBox Inventories")]
    public GameObject lootBoxInventory;
    public LootBoxInventory lootBoxInven;
    private Lootbox currentLootbox;

    [Header("Player HP UI")]
    public Slider hpSlider;
    public PlayerHP playerHP;


    [Header("Exit Timer UI")]
    public GameObject exitTimerUI;
    public TextMeshProUGUI exitTimerText;


    private bool isInventoryOpen = false;
    private bool isCamOff = false;
    private Transform lootTarget = null;
    private Camera mainCam;
    private bool canLoot = false;

    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        mainCam = Camera.main;
        lootPromptUI.gameObject.SetActive(false);
    }

    void Start()
    {
        OpenIngameCanvas();
    }

    void Update()
    {
        HandleInventoryToggle();
        UpdateLootPromptPosition();
        UpdateAmmoUI();
        UpdatePlayerHP();
        cameraController.SetCamOn(isCamOff);
    }

    public void ShowExitTimerUI()
    {
        exitTimerUI.SetActive(true);
    }

    public void UpdateExitTimerUI(float time)
    {

        exitTimerText.text = $"Extraction in {Mathf.CeilToInt(time)}";
    }

    public void HideExitTimerUI()
    {
        exitTimerUI.SetActive(false);
    }

    public void ShowExitCanvas()
    {
        InventoryManager.Instance.SaveToFile();
        ingameCanvas.enabled = false;
        inventoryCanvas.enabled = false;
        deathCanvas.enabled = false;
        exitCanvas.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void ShowDeathCanvas()
    {
        inGameInventoryLoader.ClearPlayerInventory();
        InventoryManager.Instance.SaveToFile();

        ingameCanvas.enabled = false;
        inventoryCanvas.enabled = false;
        exitCanvas.enabled = false;
        deathCanvas.enabled = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
       
    }


    private void UpdatePlayerHP()
    {

        hpSlider.value = playerHP.hp;
        hpText.text = $"HP : {Mathf.RoundToInt(playerHP.hp)}";
    }

    public void BacktoMainMenu()
    {

        SceneManager.LoadScene(0);

    }

    private void UpdateAmmoUI()
    {
        if (playerWeapon == null) return;

        switch (playerWeapon.GetCurrentWeaponType())
        {
            case PlayerWeapon.WeaponType.AK47:
                ammoText.text = $"AK47 : {playerWeapon.ak47.TotalAmmo} / {playerWeapon.GetReserveAmmo()}";
                crosshair.defaultSpread = 20;
                crosshair.maxSpread = 50;
                break;
            case PlayerWeapon.WeaponType.M3:
                ammoText.text = $"M3 : {playerWeapon.m3.TotalAmmo} / {playerWeapon.GetReserveAmmo()}";
                crosshair.defaultSpread = 100;
                crosshair.maxSpread = 130;
                break;
            default:
                ammoText.text = "NO WEAPON";
                crosshair.defaultSpread = 20;
                break;
        }
    }

    private void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInventoryOpen = !isInventoryOpen;
            ChangeCanvas();
        }

        if (Input.GetKeyDown(KeyCode.F) && canLoot)
        {
            isInventoryOpen = !isInventoryOpen;

            if (currentEnemy != null)
            {

                enemyInventory.SetActive(true);
                enemyInven.SetEnemy(currentEnemy);

                
            }
            else if (currentLootbox != null)
            {

                lootBoxInventory.SetActive(true);

                lootBoxInven.SetItems(currentLootbox.GetItems(), currentLootbox);
            }

            ChangeCanvas();
        }
    }

    private void ChangeCanvas()
    {
        if (isInventoryOpen)
        {
            OpenInventoryCanvas();
        }
        else
        {
            OpenIngameCanvas();
        }
    }

    private void OpenIngameCanvas()
    {
        ingameCanvas.enabled = true;
        inventoryCanvas.enabled = false;
        exitCanvas.enabled = false;
        deathCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCamOff = false;

        Time.timeScale = 1f;
    }

    private void OpenInventoryCanvas()
    {
        ingameCanvas.enabled = false;
        inventoryCanvas.enabled = true;
        exitCanvas.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCamOff = true;

        Time.timeScale = 0f;
    }

    private void UpdateLootPromptPosition()
    {
        if (lootTarget == null || lootPromptUI == null) return;

        Vector3 screenPos = mainCam.WorldToScreenPoint(lootTarget.position + worldOffset);
        if (screenPos.z > 0)
        {
            lootPromptUI.position = screenPos;
        }
        else
        {
            lootPromptUI.gameObject.SetActive(false);
        }
    }



    public void HideLootPrompt()
    {
        canLoot = false;
        lootTarget = null;
        currentEnemy = null;
        currentLootbox = null;

        lootPromptUI.gameObject.SetActive(false);

        if (enemyInventory != null)
            enemyInventory.SetActive(false);

        if (lootBoxInventory != null)
            lootBoxInventory.SetActive(false);
    }



    public void ShowLootPrompt(Transform target, EnemyController enemy)
    {
        if (enemy == null) return;

        canLoot = true;
        lootTarget = target;
        currentEnemy = enemy;
        currentLootbox = null;
        lootPromptUI.gameObject.SetActive(true);
    }

    public void ShowLootPrompt(Transform target, Lootbox lootbox)
    {
        if (lootbox == null) return;

        canLoot = true;
        lootTarget = target;
        currentEnemy = null;
        currentLootbox = lootbox;
        lootPromptUI.gameObject.SetActive(true);


    }
   



} 