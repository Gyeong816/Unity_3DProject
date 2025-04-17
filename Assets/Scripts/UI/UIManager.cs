using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public Canvas ingameCanvas;
    public Canvas inventoryCanvas;
    public CameraController cameraController;
    
    
    private bool isInventoryOpen = false;
    private bool isCamOff = false;
    
    void Start()
    {
        OpenIngameCanvas();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isInventoryOpen = !isInventoryOpen;
            ChangeCanvas();
        }
        
        cameraController.SetCamOn(isCamOff);
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
}
