using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BroWalkDown : MonoBehaviour {

    public Transform bottomPoint;

    public bool ShoutZone;
    public float moveSpeed;

    private Rigidbody2D myRigidBody;

    private Animator anim;
	public CollisionHandler stairwayExit;

    // Use this for initialization
    void Start () {
        ShoutZone = true;
        myRigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        if ((GameObject.Find("BrotherShoutZone") == null) && (GameManager.instance.inDialog == false))
        {
            ShoutZone = false;
            //anim.SetFloat("MoveY", -transform.position.y);
            myRigidBody.velocity = new Vector2(0, -moveSpeed);
            anim.SetFloat("MoveY", transform.position.y);

			if (this.transform.position.y <= bottomPoint.position.y)
			{
				stairwayExit.sceneToLoad = "NewHouse2F";
				Destroy(this.gameObject);
			}
        }
	}
}
