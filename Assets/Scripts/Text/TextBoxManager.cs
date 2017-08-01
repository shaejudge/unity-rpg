using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextBoxManager : MonoBehaviour {

	public GameObject textBox;
	
	//default text file that can be loaded straight to the textbox if activatetextatline has no associated text file
	public Text theText;
	
	public TextAsset textFile;
	public string[] textLines;
	
	public int currentLine;
	public int endAtLine;
	
	//public pcMovement player;
	
	public bool isActive;
	
	public bool stopPlayerMovement;
	

	// Use this for initialization
	void Start () {
		//player = FindObjectOfType<pcMovement>();
	
		if(textFile != null){
			//grabs text and splits into lines
			textLines = (textFile.text.Split('\n'));
		}
		
		if(endAtLine == 0){
			endAtLine = textLines.Length - 1;
		}
		
		if(isActive){
			EnableTextBox();
		}else{
			DisableTextBox();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isActive && Input.GetKeyDown(KeyCode.Space)){
			advanceTextBox();
		}
	}

	public void advanceTextBox()
	{
		currentLine += 1;

		Debug.Log("dialog next line: " + currentLine.ToString());

		//note that I also reordered these lines and placed the text updating line in an "else"
		//I think this would probably be better done outside an update (regular function triggered elsewhere)
		//but this works for now
		if (currentLine > endAtLine)
		{
			Debug.Log("line # out of range, closing text box");
			DisableTextBox();
		}
		else
		{
			theText.text = textLines[currentLine];
		}
	}
	
	public void EnableTextBox(){
		textBox.SetActive(true);
		theText.text = textLines[currentLine];
		Debug.Log("dialog start line: " + currentLine.ToString());
		StartCoroutine(delayActive());

		GameManager.instance.inDialog = true;
	}

	private IEnumerator delayActive()
	{
		yield return new WaitForSeconds(0.0625f);
		isActive = true;
	}
	
	public void DisableTextBox(){
		textBox.SetActive(false);
		theText.text = "";

		StartCoroutine(toggleActive());
	}
	
	public void ReloadScript(TextAsset theText){
		if(theText != null){
			textLines = new string[1];
			textLines = (theText.text.Split('\n'));
		}
	}

	private IEnumerator toggleActive()
	{
		//player is stuck for 1/8 a second
		//seems like it's not too jarring
		yield return new WaitForSeconds(0.125f);

		isActive = false;
		GameManager.instance.inDialog = false;
	}
}












