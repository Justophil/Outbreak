using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ZombieStats : MonoBehaviour {
    /*
        Zombie Types:
        0: Testing Death
        1: Normal zombie
        2: Tank Slow zombie
        3: Fast Zombie bit lower health
    */
    public static int zombieType;
    private int health;
    private int speed;
    
    void setUpStats() {
        switch(zombieType) {
            case 1:
                this.health = 150;
                this.speed = 3;
                break;
            case 2:
                this.health = 250;
                this.speed = 1;
                break;
            case 3:
                this.health = 100;
                this.speed = 5;
                break;
            default:
                this.health = 1;
                this.speed = 0;
                break;
        }
    }

    void decreaseHealth(int damage) {
        this.health -= damage;
        if(this.health <= 0) {
            // Put animation of death here
            Destroy(gameObject);
        }
    }
}