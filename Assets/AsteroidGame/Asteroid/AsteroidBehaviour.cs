using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;


// The AsteroidBehaviour holds in the information about the asteroid
public class AsteroidBehaviour : NetworkBehaviour
{
    // The _points variable can be a local private variable as it will only be used to add points to the score
    // The score itself is networked and any increase or decrease will be propagated automatically.
    [SerializeField] private int _points = 1;
    
    // The IsBig variable is Networked as it can be used to evaluate and derive visual information for an asteroid locally.
    [HideInInspector][Networked] public NetworkBool IsBig { get; set; }

    // When the asteroid gets hit by another object, this method is called to decide what to do next.
    public void HitAsteroid(PlayerRef player)
    {
        // The asteroid hit only triggers behaviour on the host.
        if (Object == null) return;
        if (Object.HasStateAuthority == false) return;
        
        // Big asteroids tell the AsteroidSpawner to spawn multiple small asteroids as it breaks up.
        if (IsBig) {
            FindObjectOfType<AsteroidSpawner>().BreakUpBigAsteroid(transform.position);
        }

        // If this hit was triggered by a projectile, the player who shot it gets points
        // The player object is retrieved via the Runner.
        if (Runner.TryGetPlayerObject(player, out var playerNetworkObject))
        {
            playerNetworkObject.GetComponent<PlayerDataNetworked>().AddToScore(_points);
        }

        Runner.Despawn(Object);
    }
}
