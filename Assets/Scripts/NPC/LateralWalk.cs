using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateralWalk : MonoBehaviour {

    /*
     * Usage:
     * This script is for making NPC(s) walk laterally between points
     * For instance, between machines (as if working) or on patrol
     * Create a holder in Unity that contains three game objects
     * the character, a left point, and a right point.
     * Drag the gameobjects for the left point & right point to script
     */

    public Transform leftPoint;
    public Transform rightPoint;

    public float moveSpeed;

    private Rigidbody2D myRigidBody;

    public bool movingRight;
    private bool facingRight = true;

    private Animator anim;


    // Use this for initialization
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("MoveX", Mathf.Abs(moveSpeed));

        if (movingRight && transform.position.x > rightPoint.position.x)
        {
            movingRight = false;
            Flip();

        }

        if (!movingRight && transform.position.x < leftPoint.position.x)
        {
            movingRight = true;
            Flip();
        }

        if (movingRight)
        {
            myRigidBody.velocity = new Vector3(moveSpeed, myRigidBody.velocity.y, 0f);
        }
        else
        {
            myRigidBody.velocity = new Vector3(-moveSpeed, myRigidBody.velocity.y, 0f);
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    void OnCollisionEnter2D(Collision2D character)
    {
        if (character.gameObject.name == "MainCharacter")
        {
            moveSpeed = 0;
            anim.enabled = false;
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
