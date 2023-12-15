using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    private Slider healthBar;
    private GameManager gameManager;
    float health = GameManager.Health;
    

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GameObject.Find("HealthBar").GetComponent<Slider>();
        gameManager = FindObjectOfType<GameManager>();

    }
    // Update is called once per frame
    void Update()
    {
        healthBar.value = health;
    }

      public void DecrementHealth() // Upon death of the player,     score decrease a good amount.
    {
        Debug.Log("Player Hit");
        healthBar.value -= 10;
        if (healthBar.value <= 0)
        {
            gameManager.LoadLevel(0);
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter(Collider other){
        if(other.tag == "Zombie"){
            DecrementHealth();
        }
    }

    
}
