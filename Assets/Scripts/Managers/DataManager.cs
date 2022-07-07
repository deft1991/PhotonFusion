using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class DataManager : MonoBehaviour, IGameManager
{
    public ManagerStatus Status { get; private set; }

    private NetworkService _networkService;
    private string _filename;
    
    public void Startup(NetworkService networkService)
    {
       Debug.Log("Data manager ProcessStartInfo...");

       _networkService = networkService;

       /*
        * Generating path to game.dat file
        */
       _filename = Path.Combine(Application.persistentDataPath, "game.dat");
       Status = ManagerStatus.Started;
    }

    public void SaveGameState()
    {
        /*
         * Dictionary for serialization
         */
        Dictionary<string, object> gameState = new Dictionary<string, object>();
        gameState.Add("inventory", Managers.Inventory.GetData());
        gameState.Add("health", Managers.Player.health);
        gameState.Add("maxHealth", Managers.Player.maxHealth);
        gameState.Add("curLevel", Managers.Mission.CurLevel);
        gameState.Add("maxLevel", Managers.Mission.MaxLevel);

        /*
         * Create file by file name
         */
        FileStream stream = File.Create(_filename);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, gameState);
        stream.Close();
    }

    public void LoadGameState()
    {
        /*
         * Load game only if we have saved data
         */
        if (!File.Exists(_filename))
        {
            Debug.Log("No saved game");
            return;
        }

        /*
         * Dictionary to place data
         */
        Dictionary<string, object> gameState;

        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = File.Open(_filename, FileMode.Open);
        gameState = formatter.Deserialize(stream) as Dictionary<string, object>;
        stream.Close();
        
        /*
         * Update all managers by needed data
         */
        Managers.Inventory.UpdateData((Dictionary<string, int>) gameState["inventory"]);
        Managers.Player.UpdateData((int) gameState["health"], (int) gameState["maxHealth"]);
        Managers.Mission.UpdateData((int) gameState["curLevel"], (int) gameState["maxLevel"]);
        Managers.Mission.RestartCurrentLevel();
    }
}
