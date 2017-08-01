using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBattle : MonoBehaviour {

	//enter these right on the inspector
	public string battleScene = "";
	[TextArea(2, 5)]
	public string battleMessage = "";

	void activateBattle()
	{
		GameManager.instance.specialBattleScene = battleScene;
		GameManager.instance.battleStartMessage = battleMessage;
		GameManager.instance.specialBattle();
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.name == "MainCharacter")
		{
			activateBattle();
		}
	} 
}
