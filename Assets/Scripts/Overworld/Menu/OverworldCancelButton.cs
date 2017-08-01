using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCancelButton : MonoBehaviour {

	public void cancelSkill()
	{
		GameManager.instance.mainMenu.skillPanelManager.deactivateTargetPanel();
	}
}
