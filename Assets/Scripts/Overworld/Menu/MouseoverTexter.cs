using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseoverTexter : MonoBehaviour {

	public string mouseoverText;
	//public Text

	// Use this for initialization
	void Start () {
		
	}

	public void showInfo()
	{
		//at some point, these references should be moved out to skip the skillPanelManager middleman
		GameManager.instance.mainMenu.skillPanelManager.activateSkillDescPanel(mouseoverText);
	}

	public void hideInfo()
	{
		GameManager.instance.mainMenu.skillPanelManager.deactivateSkillDescPanel();
	}
}
