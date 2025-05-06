using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Update = UnityEngine.PlayerLoop.Update;

public class MainmenuManager : MonoBehaviour
{
    public GameObject buttonScreen;
    public GameObject stashInventory;
    public GameObject shopInventory;
    public GameObject playerInventory;
    


    private void Start()
    {
        buttonScreen.SetActive(true);
        shopInventory.SetActive(false);
        playerInventory.SetActive(false);
        stashInventory.SetActive(false);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            buttonScreen.SetActive(true);
            shopInventory.SetActive(false);
            playerInventory.SetActive(false);
            stashInventory.SetActive(false);
        }
        
    }

    public void StartGameButton()
    {
        InventoryManager.Instance.SaveToFile();
        SceneManager.LoadScene(1);
    }
    public void OpenInventoryButton()
    {
        buttonScreen.SetActive(false);
        shopInventory.SetActive(false);
        playerInventory.SetActive(true);
        stashInventory.SetActive(true);
        
    }
    public void OpenStoreButton()
    {
        buttonScreen.SetActive(false); 
        playerInventory.SetActive(false);
        shopInventory.SetActive(true);
        stashInventory.SetActive(true);
    }
    public void QuitGameButton()
    {
        InventoryManager.Instance.SaveToFile();
        Application.Quit();
    }
}