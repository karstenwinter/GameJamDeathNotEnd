using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrap : MonoBehaviour {

    public bool zombiesToo;
    public bool deathSpawnsZombie = true;
    public GameBehaviour game;

    // check which object we are colliding with - player dies, zombie maybe.
    void OnCollisionEnter2D(Collision2D other) { 
        if (other.gameObject.tag == "Player") {
            // spieler stirbt
            game.playerDeath(deathSpawnsZombie);
        } else if(zombiesToo && other.gameObject.tag == "Zombie")  {
            // zombie stirbt
            other.gameObject.GetComponent<ZombieBodyBehaviour>().killZombie();
        }
    }
}
