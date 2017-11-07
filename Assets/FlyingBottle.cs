using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingBottle : MonoBehaviour {
    private GameBehaviour game;

    public void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameMainObj").GetComponent<GameBehaviour>();
    }

    // check when we are colliding with something
    void OnCollisionEnter2D(Collision2D other)
    {
        game.BottleHit(this.gameObject);
    }
}
