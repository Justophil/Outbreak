using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static float InitialScore; // Updated each Level, (may not be may be used)
    public static float Score = 0; // Overall Score, Synchronous with gameplay 
    public static Slider Health; // Health of player, shared with player component for UI purpose
    public static float Stamina; // Stamina of player, shared with player component for UI purpose
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
    }
    
    public static void LoadLevel(int index = 0) // Load same Level, Index = 1 to go to next level
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + index);
    }
    
    public static void IncrementScore() // Every Zombie kill, could have multiplier paarameter for special zombies
    {
        Score = Score + 50;
    }
    
    public static void DecrementScore() // Upon death of the player,     score decrease a good amount.
    {
        Score = Score - 500;
        if (Score < 0) Score = 0;
    }
}
