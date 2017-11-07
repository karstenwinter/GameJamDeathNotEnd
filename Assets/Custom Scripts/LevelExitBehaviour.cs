using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitBehaviour : MonoBehaviour
{
	public Transform target;
	public GameBehaviour game;

	private void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag == "Player") {
			game.player.transform.position = target.position;
			game.checkPoint = target.position;
			// SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
		}
	}
}