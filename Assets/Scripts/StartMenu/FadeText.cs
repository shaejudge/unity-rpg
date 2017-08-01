using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//nowhere near as fancy as the one from the wiki, but the wiki thing works on images, not text
//so writing my own here to get a similar effect
//actually breathing text, not fading only in/out
//like weak pulsation
public class FadeText : MonoBehaviour {

	public Text textThing;
	public float pauseDelay = 1f;
	public float currentDelayTimer;
	public bool increasing;

	// Use this for initialization
	void Start()
	{
		textThing = transform.GetComponent<Text>();
		currentDelayTimer = pauseDelay;
	}

	//could use a coroutine, but nah
	//just handling it all in update
	//simpler, and still fine for our purpose

	// Update is called once per frame
	void Update () {
		//while delayed, don't change alpha
		if (currentDelayTimer > 0)
		{
			currentDelayTimer -= Time.deltaTime;
		}
		else
		{
			//get current color info
			Color currentColor = textThing.color;
			//if near 1, time to go back down after waiting a second
			if (currentColor.a > 0.99f)
			{
				increasing = false;
				currentDelayTimer = pauseDelay;
			}
			else if (currentColor.a < 0.01f)
			{
				increasing = true;
				currentDelayTimer = pauseDelay;
			}

			if (increasing)
			{
				currentColor.a += 0.01f;
			}
			else
			{
				currentColor.a -= 0.01f;
			}
			textThing.color = currentColor;
		}
	}
}
