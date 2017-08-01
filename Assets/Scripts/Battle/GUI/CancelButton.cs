using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelButton : MonoBehaviour {

	public void cancelMenu()
	{
		GameObject.Find("BattleManager").GetComponent<StateMachineBattle>().cancelMenu();
	}
}
