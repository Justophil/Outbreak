using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    private GameManager gameManager;
    private NavMeshAgent navMeshAgent;

    public ZombieType zombieType;
    private int health;
    private int speed;

    void Start()
    {
        zombieType = (ZombieType)Random.Range(1, 4);
        SetUpStats();
        gameManager = GameManager.Instance;
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.speed = speed;
        }

    }

    void SetUpStats()
    {
        switch (zombieType)
        {
            case ZombieType.NormalZombie:
                health = 150;
                speed = 10;
                break;
            case ZombieType.TankSlowZombie:
                health = 250;
                speed = 8;
                break;
            case ZombieType.FastZombie:
                health = 100;
                speed = 18;
                break;
            default:
                health = 1;
                speed = 15;
                break;
        }
    }

    public void DecreaseHealth(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            // Put animation of death here
            gameManager.IncrementScore(20);
            Destroy(gameObject);
        }
    }
}