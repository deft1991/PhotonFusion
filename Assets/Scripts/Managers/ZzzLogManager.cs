using UnityEngine;
using System.Collections;

public class ZzzLogManager : MonoBehaviour, IGameManager
{
    
    public ManagerStatus Status { get; private set; }
    public void Startup(NetworkService networkService)
    {
        Debug.Log("ZzzLog manager starting..");
#if PLATFORM_STANDALONE 

        
#endif        
        Status = ManagerStatus.Started;
        Debug.Log("ZzzLogManager: started");
    }
    
    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();

    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        if (type == LogType.Exception)
            myLogQueue.Enqueue(stackTrace);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    void OnGUI() {
        GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 400, Screen.height));
        GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        GUILayout.EndArea();
    }

}