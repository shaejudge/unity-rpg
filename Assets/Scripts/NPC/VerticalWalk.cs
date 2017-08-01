using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalWalk : MonoBehaviour {

    /*
     * Usage:
     * Place top and bottom gameobjects
     * between NPC to walk vertically
     */

    public Transform topPoint;
    public Transform bottomPoint;

    public float moveSpeed;

    private Rigidbody2D myRigidBody;

    public bool movingUp;
    //private bool pause;

    private Animator anim;
    private double animTime;
    

    // Use this for initialization
    void Start () {
        myRigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {

        if (movingUp && transform.position.y > topPoint.position.y)
        {
            movingUp = false;
            anim.SetFloat("MoveY", -transform.position.y);
        }

        if (!movingUp && transform.position.y < bottomPoint.position.y)
        {
            movingUp = true;
            anim.SetFloat("MoveY", -transform.position.y);
        }

        if (movingUp)
        {
            myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, moveSpeed, 0f);
            //myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, moveSpeed);
        }
        else
        {
            myRigidBody.velocity = new Vector3(myRigidBody.velocity.x, -moveSpeed, 0f);
            //myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, -moveSpeed);
        }
    }

    void OnCollisionEnter2D(Collision2D character)
    {

        if (character.gameObject.name == "MainCharacter")
        {
            moveSpeed = 0;
            anim.enabled = false;
            //animTime = anim.GetTime();
            //anim.
           
        }

    }

    void OnCollisionExit2D(Collision2D character)
    {
        if (character.gameObject.name == "MainCharacter")
        {
            anim.enabled = true;
            moveSpeed = 3;
            
        }
    }
}
