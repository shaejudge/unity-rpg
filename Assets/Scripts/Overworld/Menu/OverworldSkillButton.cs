using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldSkillButton : MonoBehaviour {

	public BaseAction skillToPerform;
	public BasePC actor;

	public void buttonPressed()
	{
		GameManager.instance.mainMenu.skillPanelManager.selectSkill(actor, skillToPerform);
	}

	public void showSkillInfo()
	{
		string skillInfo = skillToPerform.actionCost.ToString() + " MP: " + skillToPerform.actionDesc;
		GameManager.instance.mainMenu.skillPanelManager.activateSkillDescPanel(skillInfo);
	}

	public void hideSkillInfo()
	{
		GameManager.instance.mainMenu.skillPanelManager.deactivateSkillDescPanel();
	}
}
