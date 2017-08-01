using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillButton : MonoBehaviour {

	public BaseAction skillToPerform;

	public void executeSkill()
	{
		GameObject.Find("BattleManager").GetComponent<StateMachineBattle>().selectSkill(skillToPerform);
	}

	public void showSkillInfo()
	{
		string skillInfo = skillToPerform.actionCost.ToString() + " MP: " + skillToPerform.actionDesc;
		GameObject.Find("BattleManager").GetComponent<StateMachineBattle>().activateSkillDescPanel(skillInfo);
	}

	public void hideSkillInfo()
	{
		GameObject.Find("BattleManager").GetComponent<StateMachineBattle>().deactivateSkillDescPanel();
	}
}
