using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IGameManager
{
    
    public ManagerStatus Status { get; private set; }
    public int health { get; private set; }
    public int maxHealth { get; private set; }

    private NetworkService _networkService;
    
    public void Startup(NetworkService networkService)
    {
        Debug.Log("Player manager starting..");

        _networkService = networkService;

        UpdateData(50, 100);
        
        Status = ManagerStatus.Started;
        Debug.Log("PlayerManager: started");
    }

    public void UpdateData(int health, int maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;
    }

    public void ChangeHealth(int value)
    {
        health += value;
        if (health > maxHealth)
        {
            health = maxHealth;
        } else if (health < 0)
        {
            health = 0;
        }

        if (health == 0)
        {
            Messenger.Broadcast(GameEvent.LEVEL_FAILED);
        }
        Debug.Log( "Health: " + health + "/" + maxHealth);
        
        Messenger.Broadcast(GameEvent.HEALTH_UPDATED);
    }

    public void Respawn()
    {
        // todo move to params
        UpdateData(50, 100);
    }
}
