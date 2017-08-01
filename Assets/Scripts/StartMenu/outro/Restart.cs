using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour {

	void Update()
	{
		if (Input.GetKeyDown("space") || Input.GetKeyDown("return"))
		{
			//have to nuke that music too, because it got a dontdestroyonload
			//nuke everything
			Destroy(GameObject.Find("FinalMusic"));
			SceneManager.LoadScene("StartMenu");
		}
	}
}
