using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossText : MonoBehaviour {

    public TextAsset theText;

    public int startLine;
    public int endLine;
    public int tempStart;
    public int tempEnd;

    public TextBoxManager theTextBox;

    public bool destroyWhenActivated;

    public bool requireButtonPress;
    private bool waitingForPress;
    //waitingForPress means in range and waiting for player to activate with spacebar

    // Use this for initialization
    void Start()
    {
        theTextBox = GameManager.instance.transform.Find("TextBoxManager").GetComponent<TextBoxManager>();
		tempStart = startLine;
        tempEnd = endLine;
    }

    // Update is called once per frame
    void Update()
    {
        //if textbox isn't already active, the in-game menu isn't active, ready and waiting for spacebar press, and spacebar is pressed
        if (!theTextBox.isActive && !GameManager.instance.mainMenu.isActive && waitingForPress && (Input.GetKeyDown(KeyCode.Space)))
        {


            theTextBox.ReloadScript(theText);
            theTextBox.currentLine = startLine;
            theTextBox.endAtLine = endLine;
            theTextBox.EnableTextBox();

            if (destroyWhenActivated)
            {
                Destroy(gameObject);
            }

            startLine++;
            endLine++;

            if (startLine == 8)
            {
                startLine = tempStart;
                endLine = tempEnd;
            }

        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "MainCharacter")
        {
            if (requireButtonPress)
            {
                waitingForPress = true;
                return;
            }

            theTextBox.ReloadScript(theText);
            theTextBox.currentLine = startLine;
            theTextBox.endAtLine = endLine;
            theTextBox.EnableTextBox();

            if (destroyWhenActivated)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.name == "MainCharacter")
        {
            waitingForPress = false;
        }
    }
}
