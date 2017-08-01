using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadButton : MonoBehaviour {

	//again just a wrapper to find and use the game manager
	public void loadGame()
	{
		GameManager.instance.loadGame();
	}
}
