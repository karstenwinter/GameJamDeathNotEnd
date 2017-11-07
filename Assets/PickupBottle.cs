using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupBottle : MonoBehaviour {
    private GameBehaviour game;
    public bool pickupGone = true;

    void Start()
    {
        game = GameObject.FindGameObjectWithTag("GameMainObj").GetComponent<GameBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            game.givePlayerBottle();
            if(pickupGone) this.gameObject.SetActive(false);
        }
    }
}
