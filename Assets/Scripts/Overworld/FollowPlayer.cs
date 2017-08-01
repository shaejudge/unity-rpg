using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform target;
    public float speed = 2f;
    private float minDistance = 1f;
    private float range;
    void Update()
    {
        range = Vector2.Distance(transform.position, target.position);

        if (range > minDistance)
        {
            Debug.Log(range);

                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
    }

}
