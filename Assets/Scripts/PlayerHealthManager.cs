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
        healthBar.value = health;
        Debug.Log("Starting Health: " + health);

    }
    // Update is called once per frame
    void Update()
    {
        healthBar.value = health;
    }

    public void DecrementHealth() // Upon death of the player,     score decrease a good amount.
    {
        Debug.Log("Player Hit");
        health -= 10;
        if (health <= 0)
        {
            gameManager.LoadLevel(0);
            Destroy(gameObject);
        }
    }

    // public void OnTriggerEnter(Collider other){
    //     if(other.tag == "Zombie"){
    //         DecrementHealth();
    //     }
    // }

    
}
