using UnityEngine;
using System;
using System.Collections;
using UnityStandardAssets._2D;

public class ZombieSpawnBehaviour : MonoBehaviour
{
	public GameBehaviour game;
	public int count = 1;
	public float seconds = 1;
	private float time;
    public bool zombieInvincible = false;

	void Update()
	{
		if (time <= 0) {
			time = seconds;
			if (count > 0) {
				game.NewZombie (transform, zombieInvincible);
				count--;
			}
		}
		time -= Time.deltaTime;
	}
}
