using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    
    
    public void StartGameButton()
    {
        SceneManager.LoadScene(1);
    }
    public void OpenInventoryButton()
    {
        
    }
    public void OpenStoreButton()
    {
       
    }
    public void QuitGameButton()
    {
        Application.Quit();
    }
}
