using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour {
    public void LoadByIndex(int sceneIndex)
    {
        //PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(sceneIndex);
    }

	//adding more to this function since basic Unity provided menu isn't quite enough
	//Unity provided function is actually above one, but still
	public void LoadByName(string sceneName)
	{
		//PlayerPrefs.DeleteAll();
		GameManager.instance.startNewGame(sceneName);
	}
}
