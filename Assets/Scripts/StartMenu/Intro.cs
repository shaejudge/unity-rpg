using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour {

    public GameObject menu; // Assign in inspector
    private bool isShowing;

    // Use this for initialization
    void Start () {
        
        menu.SetActive(!isShowing);
        StartCoroutine(Wait());
        if (!isShowing)
        {
            menu.SetActive(isShowing);
        }

    }

    IEnumerator Wait()
    {
        print(Time.time);
        yield return new WaitForSeconds(4);
        print(Time.time);
    }

    // Update is called once per frame
    void Update () {
        if ((Input.GetKeyDown("space") || (Input.GetKeyDown("return"))) && !isShowing)
        {
            isShowing = !isShowing;
            menu.SetActive(isShowing);
        }
    }
}
