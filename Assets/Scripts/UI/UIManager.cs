using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Canvas")]
    public Canvas ingameCanvas;
    public Canvas inventoryCanvas;

    [Header("Camera Control")]
    public CameraController cameraController;

    [Header("Loot Prompt UI")]
    public RectTransform lootPromptUI; // "F" 표시용 UI 
    public Vector3 worldOffset = new Vector3(0, 2f, 0); // 시체 머리 위 위치

    private Enemy currentEnemy;

    public GameObject enemyInventory;
    public EnemyInventory enemyInven;


    private bool isInventoryOpen = false;
    private bool isCamOff = false;

    private Transform lootTarget = null; // 현재 UI가 따라갈 대상
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
        cameraController.SetCamOn(isCamOff);
    }

    private void HandleInventoryToggle()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInventoryOpen = !isInventoryOpen;
            ChangeCanvas();
        }
        if (Input.GetKeyDown(KeyCode.F)&& canLoot)
        {
            isInventoryOpen = !isInventoryOpen;
            enemyInventory.SetActive(true);
            enemyInven.SetEnemy(currentEnemy);


            if (!currentEnemy.vestSpawned) 
            {
                enemyInven.SpawnVest1();
                currentEnemy.SetVestSpawned(true); 
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCamOff = false;
    }

    private void OpenInventoryCanvas()
    {
        ingameCanvas.enabled = false;
        inventoryCanvas.enabled = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCamOff = true;
    }

    private void UpdateLootPromptPosition()
    {
        if (lootTarget == null || lootPromptUI == null)
        {
            return;
        }

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

    public void ShowLootPrompt(Transform target, Enemy enemy)
    {
        canLoot = true;
        lootTarget = target;
        currentEnemy = enemy; 
        lootPromptUI.gameObject.SetActive(true);
    }

    public void HideLootPrompt()
    {
        canLoot = false;
        lootTarget = null;
        lootPromptUI.gameObject.SetActive(false);
        enemyInventory.SetActive(false);
    }

   
}
