using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MissionManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }
    
    public int CurLevel { get; private set; }
    public int MaxLevel { get; private set; }

    private NetworkService _networkService;
    
    public void Startup(NetworkService networkService)
    {
        Debug.Log("Mission manager starting...");

        _networkService = networkService;

        UpdateData(0, 1);

        Status = ManagerStatus.Started;
    }

    public void UpdateData(int curLevel, int maxLevel)
    {
        CurLevel = curLevel;
        MaxLevel = maxLevel;
    }

    public void GoToNext()
    {
        if (CurLevel < MaxLevel)
        {
            CurLevel++;
            string name = "Level " + CurLevel;
            Debug.Log("Loading: " + name);
            SceneManager.LoadScene(name);
        }
        else
        {
            Debug.Log("Last level");
            Messenger.Broadcast(GameEvent.GAME_COMPLETE);
        }
    }

    public void ReachObjective()
    {
        /*
         * Here we can handle multiple goals
         */
        Messenger.Broadcast(GameEvent.LEVEL_COMPLETE);
    }

    public void RestartCurrentLevel()
    {
        string name = "Level" + CurLevel;
        Debug.Log("Loading " + name);
        SceneManager.LoadScene(name);
    }
}
