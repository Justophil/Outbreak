using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieStats : MonoBehaviour
{
    /*
        Zombie Types:
        0: Testing Death
        1: Normal zombie
        2: Tank Slow zombie
        3: Fast Zombie bit lower health
    */
    public enum ZombieType
    {
        TestingDeath = 0,
        NormalZombie = 1,
        TankSlowZombie = 2,
        FastZombie = 3
    }

    public ZombieType zombieType;
    private int health;
    private int speed;

    void Start()
    {
        SetUpStats();
    }

    void SetUpStats()
    {
        switch (zombieType)
        {
            case ZombieType.NormalZombie:
                health = 150;
                speed = 3;
                break;
            case ZombieType.TankSlowZombie:
                health = 250;
                speed = 1;
                break;
            case ZombieType.FastZombie:
                health = 100;
                speed = 5;
                break;
            default:
                health = 1;
                speed = 0;
                break;
        }
    }

    public void DecreaseHealth(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Put animation of death here
            Destroy(gameObject);
        }
    }
}