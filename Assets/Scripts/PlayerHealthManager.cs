using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    public Slider healthBar;
    float health = GameManager.Health;
    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = health;
    }

      public void DecrementHealth() // Upon death of the player,     score decrease a good amount.
    {
        healthBar.value -= 50;
        if (healthBar.value <= 0)
        {
            gameManager.LoadLevel(0);
            Destroy(gameObject);
        }

    }

    
}
