using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MusicNoStop : MonoBehaviour {

    public string changeSongScene;
    public static bool AudioBegin = false;
    void Awake()
    {
        if (!AudioBegin)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Play();
            DontDestroyOnLoad(gameObject);
            AudioBegin = true;
        }
		else
		{
			//else self destruct
			Destroy(gameObject);
		}
    }
    

    void Update()
    {
        if (SceneManager.GetActiveScene().name == changeSongScene)
        {
			Destroy(gameObject);
            AudioSource audio = GetComponent<AudioSource>();
            audio.Stop();
            AudioBegin = false;
        }
    }
}
