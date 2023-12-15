using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static float InitialScore; // Updated each Level, (may not be may be used)
    public static int Score = 0; // Overall Score, Synchronous with gameplay 
    public static float Health = 100; // Health of player, shared with player component for UI purpose
    public static float Stamina; // Stamina of player, shared with player component for UI purpose
    public static int CurrentLevel = 1;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(Instance);
    }
    
    public void LoadLevel(int index = 0) // Load same Level, Index = 1 to go to next level
    {
        CurrentLevel = index;
        SceneManager.LoadScene(index);
    }
    
    public void IncrementScore(int score) // Every Zombie kill, could have multiplier paarameter for special zombies
    {
        Score = Score + score;
        if (Score == 400)
        {
            LoadLevel(CurrentLevel + 1);
            // SceneManager.LoadScene(CurrentLevel + 1);
        }
    }
    
    public void DecrementScore() // Upon death of the player,     score decrease a good amount.
    {
        Score = Score - 500;
        if (Score < 0) Score = 0;
    }

}
