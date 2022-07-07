using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{
    [SerializeField] private Ball prefabBall;
    [SerializeField] private PhysxBall prefabPhysxBall;
    
    [Networked(OnChanged = nameof(OnBallSpawned))]
    public NetworkBool spawned { get; set; }
    
    private Material _material;
    Material Material
    {
        get
        {
            if(_material==null)
                _material = GetComponentInChildren<MeshRenderer>().material;
            return _material;
        }
    }

    [Networked] private TickTimer delay { get; set; }


    private Vector3 _forward;
    private NetworkCharacterControllerPrototype _cc;
    private TextMeshProUGUI _messages;


    private void Awake()
    {
        _cc = GetComponent<NetworkCharacterControllerPrototype>();
        _forward = transform.forward;
    }

    // private void Update()
    // {
    //     if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.R))
    //     {
    //         RPC_SendMessage("Hey Mate!");
    //     }
    // }

    /**
     * That is RPC call
     * 
     */
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void RPC_SendMessage(string message, RpcInfo info = default)
    {
        if (_messages == null)
            _messages = FindObjectOfType<TextMeshProUGUI>();
        if(info.IsInvokeLocal)
            message = $"You said: {message}\n";
        else
            message = $"Some other player said: {message}\n";
        _messages.text += message;
    }

    /**
     * FixedUpdateNetwork gets called on every simulation tick.
     * This can happen multiple times per rendering frame
     * as Fusion applies an older confirmed network state
     * and then re-simulates from that tick
     * all the way up to the currently (predicted) local tick.
    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            data.direction.Normalize();
            _cc.Move(5 * data.direction * Runner.DeltaTime);

            SpawnBall(data);
        }
    }
    
    /**
     * Run after all simulations
     *
     * This is done in Render() rather than Update()
     * because it is guaranteed to run after FixedUpdateNetwork()
     * and it uses Time.deltaTime rather than Runner.DeltaTime
     * because it is running in Unity's render loop and not as part of the Fusion simulation.
     */
    public override void Render()
    {
        Material.color = Color.Lerp(Material.color, Color.red, Time.deltaTime );
    }

    /**
     * Spawn ball if click button
     */
    private void SpawnBall(NetworkInputData data)
    {
        if (data.direction.sqrMagnitude > 0)
        {
            _forward = data.direction;
        }

        /*
         * Spawn only if delay expired
         * avoid multiple balls
         */
        if (delay.ExpiredOrNotRunning(Runner))
        {
            if ((data.buttons & NetworkInputData.MOUSEBUTTON1) != 0)
            {
                delay = TickTimer.CreateFromSeconds(Runner, 0.5f);
                Runner.Spawn(prefabBall,
                    transform.position + _forward, Quaternion.LookRotation(_forward),
                    Object.InputAuthority, (runner, o) =>
                    {
                        // Initialize the Ball before synchronizing it
                        o.GetComponent<Ball>().Init();
                    });
                spawned = !spawned;
            }
            else if ((data.buttons & NetworkInputData.MOUSEBUTTON2) != 0)
            {
                delay = TickTimer.CreateFromSeconds(Runner, .5f);
                Runner.Spawn(prefabPhysxBall,
                    transform.position + _forward,
                    Quaternion.LookRotation(_forward),
                    Object.InputAuthority, (runner, o) =>
                    {
                        // Initialize the Ball before synchronizing it
                        o.GetComponent<PhysxBall>().Init(10 * _forward);
                    });
                spawned = !spawned;
            }
        }
    }
    
    public static void OnBallSpawned(Changed<Player> changed)
    {
        /*
         * We have access to old properties
         *
         * var newValue = changed.Behaviour.someNetworkedProperty;
         * changed.LoadOld();
         * var oldValue = changed.Behaviour.someNetworkedProperty;
         */
        
        changed.Behaviour.Material.color = Color.white;
    }

}