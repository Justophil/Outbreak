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

    }

    public void OnGameStart(){
        gameManager.LoadLevel(1); // Loading First level
        //  SceneManager.LoadScene("Valley"); // Loading First level

    }
    public void OnGameExit(){
        Application.Quit(); // Exit Game
    }
}
