using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameScript : MonoBehaviour {

	// Use this for initialization
	public void StartGame()
    {
        Application.LoadLevel("Scene1");
    }

    // Use this for initialization
    public void ToTitleScreen()
    {
        Application.LoadLevel("startscreen");
    }
}
