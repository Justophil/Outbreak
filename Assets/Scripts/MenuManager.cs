using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void OnGameStart(){
         // GameManager.LoadLevel(1); // Loading First level
         SceneManager.LoadScene(1); // Loading First level

    }
    public void OnGameExit(){
        Application.Quit(); // Exit Game
    }
}
