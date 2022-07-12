using UnityEngine;
using System.Collections;

public class JoysticManager : MonoBehaviour, IGameManager
{
    [SerializeField] private GameObject joysticCanvas;
    public ManagerStatus Status { get; private set; }

    public void Startup(NetworkService networkService)
    {
        Debug.Log("JoysticManager manager starting..");

        GameObject joysticCanvasGameObject = Instantiate(joysticCanvas);
        DontDestroyOnLoad(joysticCanvasGameObject);
        
        Status = ManagerStatus.Started;

        Debug.Log("JoysticManager: started");
    }
}