using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets._2D;

public class GameBehaviour : MonoBehaviour {

	public PlatformerCharacter2D player;
	public GameObject zombieDefault;
	public Vector3 startPos;
	public Vector3 checkPoint { get; set; }
	public int maxZombieCount = 64;
    public bool hasBottleHitPosition;
    public Vector3 bottleHitPosition;
    private float bottleTime;
    private bool preventNextSpawn = false;

	List<GameObject> zombies = new List<GameObject> ();

    public void BottleHit(GameObject bottle)
    {
        bottleHitPosition = bottle.transform.position;
        hasBottleHitPosition = true;
        bottleTime = 12;
    }


    private float respawnTimer;

	Transform playerTransform;

	void Start () {
        hasBottleHitPosition = false;
        playerTransform = player.gameObject.transform;
		startPos = playerTransform.position;
	}
	
	void Update () {
        respawnTimer -= Time.deltaTime;
        bottleTime -= Time.deltaTime;
        if (bottleTime < 0) hasBottleHitPosition = false;
        if(player.isDying && respawnTimer < 0)
        {
            Restart();
        }
	}

    public void givePlayerBottle()
    {
        player.hasBottle = true;
        player.transform.Find("PlayerCharacter").Find("bottle").gameObject.SetActive(true);
    }

    public bool isPlayerDeath()
    {
        return player.isDying;
    }

    public void playerDeath(bool spawnZombie = true)
    {
        player.isDying = true;
        player.hasBottle = false;
        player.transform.Find("PlayerCharacter").Find("bottle").gameObject.SetActive(false);
        player.transform.Find("PlayerCharacter").GetComponent<Animator>().SetBool("death", true);
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
        respawnTimer = 2f;
        preventNextSpawn = !spawnZombie;
    }

	public void NewZombie(Vector3 position, Quaternion rotation, bool invincible = false) {
		if (zombies.Count >= maxZombieCount) {
			return;
		}
		var newZombie = Instantiate (zombieDefault, position, rotation) as GameObject;
		var zombie = newZombie.GetComponent<ZombieBodyBehaviour> ();
		newZombie.SetActive (true);
        zombie.setInvincible(invincible);

		zombies.Add (newZombie);

		while (zombies.Count > maxZombieCount) {
			zombies.RemoveAt (0);
		}
	}

	public void NewZombie(Transform transform, bool invincible = false) {
		NewZombie(transform.position, transform.rotation, invincible);
	}
	public void Restart()
	{
        player.isDying = false;
        player.transform.Find("PlayerCharacter").GetComponent<Animator>().SetBool("death", false);
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;

        

		

		for (int i = 0; i < zombies.Count; i++) {
			GameObject zomb = zombies [i];
			var kil = zomb.activeInHierarchy && zomb.GetComponent<ZombieBodyBehaviour> ().killed;
			if (kil) {
				zombies.RemoveAt (i);
				zomb.SetActive(false);
				i--;
				Destroy (zomb);
			}
		}
        if (!preventNextSpawn) {
            Vector3 spawnZombiePos = playerTransform.position;
            spawnZombiePos.y -= 0.4f;
            NewZombie (spawnZombiePos, playerTransform.rotation, false);
        }
        preventNextSpawn = false;

        playerTransform.position = checkPoint;
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        player.GetComponent<Rigidbody2D>().angularVelocity = 0;
    }
}
