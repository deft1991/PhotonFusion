using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
 * Required components
 */
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(InventoryManager))]
[RequireComponent(typeof(WeatherManager))]
[RequireComponent(typeof(ImagesManager))]
[RequireComponent(typeof(AudioManager))]
[RequireComponent(typeof(MissionManager))]
[RequireComponent(typeof(DataManager))]
[RequireComponent(typeof(ZzzLogManager))]
[RequireComponent(typeof(JoysticManager))]
[RequireComponent(typeof(BasicSpawnerManager))]
public class Managers : MonoBehaviour
{
    public static PlayerManager Player { get; private set; }
    public static InventoryManager Inventory { get; private set; }
    public static WeatherManager Weather { get; private set; }
    public static ImagesManager Images { get; private set; }
    public static AudioManager Audio { get; private set; }
    public static MissionManager Mission { get; private set; }
    public static DataManager Data { get; private set; }
    public static ZzzLogManager ZzzLog { get; private set; }
    public static JoysticManager Joystic { get; private set; }
    public static BasicSpawnerManager BasicSpawner { get; private set; }

    /*
     * List of all IGameManagers
     */
    private List<IGameManager> _startSequence;

    /**
     * Executes before start
     * uses for initialization before all other modules
     */
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        
        Player = GetComponent<PlayerManager>();
        Inventory = GetComponent<InventoryManager>();
        Weather = GetComponent<WeatherManager>();
        Images = GetComponent<ImagesManager>();
        Audio = GetComponent<AudioManager>();
        Mission = GetComponent<MissionManager>();
        Data = GetComponent<DataManager>();
        ZzzLog = GetComponent<ZzzLogManager>();
        Joystic = GetComponent<JoysticManager>();
        BasicSpawner = GetComponent<BasicSpawnerManager>();

        _startSequence = new List<IGameManager>();
        _startSequence.Add(Player);
        _startSequence.Add(Inventory);
        _startSequence.Add(Weather);
        _startSequence.Add(Images);
        _startSequence.Add(Audio);
        _startSequence.Add(Mission);
        _startSequence.Add(ZzzLog);
        _startSequence.Add(Joystic);
        _startSequence.Add(BasicSpawner);
        /*
         * MUST be last manager in loading
         */
        _startSequence.Add(Data);

        StartCoroutine(StartupManagers());
    }

    private IEnumerator StartupManagers()
    {
        NetworkService networkService = new NetworkService();
        foreach (IGameManager gameManager in _startSequence)
        {
            gameManager.Startup(networkService);
        }

        yield return null;

        int numModules = _startSequence.Count;
        int numReady = 0;

        /*
         * Do it again and again while initializing all modules
         */
        while (numReady < numModules)
        {
            int lastReady = numReady;
            numReady = 0;
            foreach (IGameManager gameManager in _startSequence)
            {
                if (ManagerStatus.Started == gameManager.Status)
                {
                    numReady++;
                }
            }

            if (numReady > lastReady)
            {
                Debug.Log("Progress: " + numReady + "/" + numModules);
                Messenger<int, int>.Broadcast(StartupEvent.MANAGERS_PROGRESS, numReady, numModules);
            }

            /*
             * Stop on one tick before the next check
             */
            yield return null;
        }
        Debug.Log("All managers started up");
        Messenger.Broadcast(StartupEvent.MANAGERS_STARTED);
    }
}