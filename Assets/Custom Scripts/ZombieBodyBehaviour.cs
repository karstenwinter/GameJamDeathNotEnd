using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;
using System;

public class ZombieBodyBehaviour : MonoBehaviour {
	private GameBehaviour game;

	public bool killed { get; private set; }
    public float headJumpSpeed;
    private bool isAttacking;
    private bool isInvincible = false;

	void Start () {
        //	game.Active = true;
        killed = false;
        isAttacking = false;

        game = GameObject.FindGameObjectWithTag("GameMainObj").GetComponent<GameBehaviour>();
    }
	
	void Update () {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // player is too close, let's kill that bastard!
		if((player.transform.position - this.transform.position).sqrMagnitude < 1.82 && 
            !isAttacking && !game.isPlayerDeath() && !killed)
        {
            transform.Find("PlayerCharacter").GetComponent<Animator>().SetTrigger("attack");
            game.playerDeath();
            
            isAttacking = true;
            return;
        }

        // player is far away? let's go get him!
        if (!game.isPlayerDeath() && !killed)
        {
            float zombieSpeed = 0.1f;
            
            if (game.hasBottleHitPosition)
            {
                GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>().Move(-((transform.position - game.bottleHitPosition).normalized.x) * zombieSpeed * 2, false, false);
            }
            else if(Mathf.Abs((transform.position - player.transform.position).x) < 12)
            {
                GetComponent<UnityStandardAssets._2D.PlatformerCharacter2D>().Move(-((transform.position - player.transform.position).normalized.x) * zombieSpeed, false, false);
            }
        }
        
    }

    void OnTriggerEnter2D(Collider2D other)
	{
        // only check for the box collider, not the circle collider
        if (other is BoxCollider2D) return;

		if (other.tag == "Player" && !killed)
		{
			Vector3 myPos = transform.position;
			Vector3 otherPos = other.transform.position;
			
			//
			bool fromAbove = myPos.y < (otherPos.y - 0.2);

            // zombie auf den kopf gesprungen
			if(fromAbove) {
                // sprung-impuls geben
                 other.GetComponent<Rigidbody2D>().velocity = new Vector2(other.GetComponent<Rigidbody2D>().velocity.x, 15);

                // zombie stirbt
                if(!isInvincible) killZombie();
                
			} else {
                // von der seite: spieler stirbt
				game.Restart();
			}
		}
	}

    internal void setInvincible(bool invincible)
    {
        isInvincible = invincible;
    }

    public void killZombie()
    {
        killed = true;
        GetComponent<CapsuleCollider2D>().enabled = killed;
        GetComponent<BoxCollider2D>().enabled = !killed;
        GetComponent<CircleCollider2D>().enabled = !killed;
        transform.Find("PlayerCharacter").GetComponent<Animator>().SetBool("isDeath", killed);
    }
}
