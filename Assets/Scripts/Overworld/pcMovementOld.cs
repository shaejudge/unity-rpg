/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pcMovementOld : MonoBehaviour {

	// Use this for initialization
	void Start () {using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pcMovement : MonoBehaviour {

	public float moveSpeed = 5f;

	private Vector2 currentPosition;
	private Vector2 previousPosition;

	private Animator animator;
	private Vector2 lastMove;

	//these floats won't be adjusted by other entities, but just made public for debugging
	public float moveX;
	public float moveY;
	public float currentSpeed;

	private void Awake()
	{
		animator = this.gameObject.transform.FindChild("Sprite").GetComponent<Animator>();
	}

	// Use this for initialization
	void Start () {
		//if a spawn point is noted, spawn there
		if (GameManager.instance.nextSpawnPoint != "")
		{
			GameObject spawnPoint = GameObject.Find(GameManager.instance.nextSpawnPoint);
			//casting to Vector2 because the 3d objects in 2d world have inconsistent z-depth right now
			transform.position = (Vector2)spawnPoint.transform.position;
			GameManager.instance.nextSpawnPoint = "";
		}
		else
		{
			transform.position = GameManager.instance.nextCharPosition;
			GameManager.instance.nextCharPosition = Vector2.zero;
		}

		//this really isn't the best way to handle this
		if (GameObject.Find("ATTACH_CAMERA"))
		{
			//I don't have spaces in my object names, but "Main Camera" is a standard Unity name
			GameObject camera = GameObject.Find("Main Camera");
			camera.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10f);
			camera.transform.SetParent(this.transform);
		}
	}

	// Update is called once per frame
	//fixed update not tied to framerate
	//regular update was removed

	//fixed update not tied to framerate
	void FixedUpdate ()
	{
		moveX = Input.GetAxisRaw("Horizontal");
		moveY = Input.GetAxisRaw("Vertical");


		//this bool is now set early on, for good reason
		//see the later comment on currentPosition/previousPosition comparisons
		//in short, the slight inaccuracy in float equality comparisons was what caused the animation oddity
		GameManager.instance.isWalking = false;

		//if menu active, we just zero out the movement and let things continue as if axes were both zero
		if (GameManager.instance.mainMenu.isActive)
		{
			moveX = 0f;
			moveY = 0f;
		}

		Vector2 movement = new Vector2(moveX, moveY);
		//no superspeed diagonals
		movement.Normalize();

		currentSpeed = movement.magnitude;

		GetComponent<Rigidbody2D>().velocity = movement * moveSpeed;

		currentPosition = transform.position;

		//keep from walking "into" walls when moving diagonally
		//the position comparison here shouldn't be used for determining the isWalking bool
		//that's what caused the animation-stop weirdness
		if (Mathf.Abs(currentPosition.x - previousPosition.x) < 0.01f && Mathf.Abs(currentPosition.y - previousPosition.y) > 0.01f)
		{
			moveX = 0f;
		}
		else if (Mathf.Abs(currentPosition.y - previousPosition.y) < 0.01f && Mathf.Abs(currentPosition.x - previousPosition.x) > 0.01f)
		{
			moveY = 0f;
		}

		if (Mathf.Abs(moveX) > 0.5f)
		{
			lastMove = new Vector2(moveX, 0f);
			GameManager.instance.isWalking = true;
		}
		else if (Mathf.Abs(moveY) > 0.5f)
		{
			lastMove = new Vector2(0f, moveY);
			GameManager.instance.isWalking = true;
		}

		animator.SetFloat("MoveX", moveX);
		animator.SetFloat("MoveY", moveY);
		animator.SetFloat("LastMoveX", lastMove.x);
		animator.SetFloat("LastMoveY", lastMove.y);
		animator.SetBool("PlayerMoving", GameManager.instance.isWalking);

		previousPosition = currentPosition;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "AreaChange")
		{
			CollisionHandler ch = other.gameObject.GetComponent<CollisionHandler>();
			GameManager.instance.nextSpawnPoint = ch.spawnPointName;
			GameManager.instance.loadScene(ch.sceneToLoad);
		}
		else if (other.tag == "DangerZone")
		{
			RegionData region = other.gameObject.GetComponent<RegionData>();
			GameManager.instance.currentRegion = region;
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "DangerZone")
		{
			GameManager.instance.inDangerZone = true;
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "DangerZone")
		{
			GameManager.instance.inDangerZone = false;
		}
	}
}

		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
*/