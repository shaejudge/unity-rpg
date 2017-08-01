using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour {

    public Transform walkToPoint;

    public float moveSpeed;

    private Rigidbody2D docRigidBody;
    private Animator anim;

    //private bool moving = true;
	public GameObject extraColliders;


    // Use this for initialization
    void Start()
    {
        docRigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if ((GameObject.Find("PCLab_door") == null))
        {
            docRigidBody.velocity = new Vector2(0f, -moveSpeed);
            anim.SetFloat("MoveY", -transform.position.y);
			extraColliders.SetActive(true);

        }

        if (this.transform.position.y <= walkToPoint.position.y)
        {
            docRigidBody.velocity = new Vector2(0, 0);
            anim.enabled = false;
			extraColliders.SetActive(false);
		}
    }

}
