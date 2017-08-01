using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedActivation : MonoBehaviour {

	public float secondObjectDelay = 0f;
	public GameObject secondObject; // Assign in inspector
	private bool isShowing;
	public bool isSprite;

	// Use this for initialization
	void Start()
	{
		secondObject.SetActive(false);
		StartCoroutine(Wait());
	}

	IEnumerator Wait()
	{
		yield return new WaitForSeconds(secondObjectDelay);

		if (isSprite)
		{
			//the sprite renderer disabling at first keeps the object from "flashing" in on activation
			secondObject.GetComponent<SpriteRenderer>().enabled = false;
		}
		//if not sprite, whatever
		//we only need to worry about flashing for a sprite, and we're not using this with that many things
		//(the text will start at 0 alpha, so not an issue)

		secondObject.SetActive(true);
	}
}
