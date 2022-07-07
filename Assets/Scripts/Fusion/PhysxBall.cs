using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PhysxBall : NetworkBehaviour
{

    [Networked] private TickTimer LifeTimer { get; set; }

    /**
     * Call it after prefab Instantiated but before sync
     */
    public void Init(Vector3 forward)
    {
        LifeTimer = TickTimer.CreateFromSeconds(Runner, 5f);
        GetComponent<Rigidbody>().velocity = forward;
    }

    public override void FixedUpdateNetwork()
    {
        if (LifeTimer.ExpiredOrNotRunning(Runner))
        {
           Runner.Despawn(Object);
        }
    }
}
