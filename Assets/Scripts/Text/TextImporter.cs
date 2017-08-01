using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextImporter : MonoBehaviour {

	public TextAsset textFile;
	public string[] textLines;

	// Use this for initialization
	void Start () {
		
		if(textFile != null){
				//grabs text and splits into lines
				textLines = (textFile.text.Split('\n'));
				
		}
		
	}
	
	// Update is called once per frame
	/*
	void Update () {
		
	}
	*/
}
