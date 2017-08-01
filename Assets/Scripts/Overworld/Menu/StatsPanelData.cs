using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsPanelData : MonoBehaviour {

	public Text charName;

	public Text level;
	public Text experience;

	public Text HP;
	public Text MP;

	public Text strength;
	public Text magic;
	public Text defense;
	public Text magdef;
	public Text agility;
	public Text luck;

	public BasePC gmStats;

	public void updateStats()
	{
		charName.text = gmStats.charName.ToString();

		//level and experience have combined labels + values, no real reason for this
		//just thought it looked fine as is without splitting into two differently aligned text boxes
		level.text = "LVL " + gmStats.level.ToString();
		experience.text = "EXP " + gmStats.experience.ToString();

		HP.text = gmStats.currentHP.ToString() + "/" + gmStats.maxHP.ToString();
		MP.text = gmStats.currentMP.ToString() + "/" + gmStats.maxMP.ToString();

		strength.text = gmStats.strength.ToString();
		magic.text = gmStats.magic.ToString();
		defense.text = gmStats.defense.ToString();
		magdef.text = gmStats.magdef.ToString();
		agility.text = gmStats.agility.ToString();
		luck.text = gmStats.luck.ToString();
	}

	//we could add functions to update individual stats, but probably not necessary
	//I don't think the full stats update for a heal would be too jarring, assuming Unity works fast

	//second thought, this seems like a good idea to extend functionality for skill panel without creating new class
	public void updateMP()
	{
		MP.text = gmStats.currentMP.ToString() + "/" + gmStats.maxMP.ToString();
	}

	public void updateHP()
	{
		HP.text = gmStats.currentHP.ToString() + "/" + gmStats.maxHP.ToString();
	}
}
