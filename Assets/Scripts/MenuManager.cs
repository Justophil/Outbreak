using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private GameManager gameManager;

 
    private void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("GameManager instance is null! Make sure GameManager is properly initialized.");
        }
        else
        {
            Debug.Log("GameManager instance accessed!");
        }

        Cursor.lockState = CursorLockMode.None;
    }

    public void OnGameStart(){
        // gameManager.LoadLevel(1); // Loading First level
        SceneManager.LoadScene(1); // Loading First level

    }
    public void OnGameExit(){
        Application.Quit(); // Exit Game
    }

    public void Controls(){
        SceneManager.LoadScene("Controls");
    }

    public void BackBtn(){
        SceneManager.LoadScene(0);
    }
}
