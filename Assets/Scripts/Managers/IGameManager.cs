using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameManager
{
    ManagerStatus Status { get; }

    void Startup(NetworkService networkService);
}
