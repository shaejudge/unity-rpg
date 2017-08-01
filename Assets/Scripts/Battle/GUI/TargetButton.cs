using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetButton : MonoBehaviour {

	public GameObject target;

	public void selectEnemy()
	{
		GameObject.Find("BattleManager").GetComponent<StateMachineBattle>().selectTarget(target);
	}

	public void selectPCTarget()
	{
		GameObject.Find("BattleManager").GetComponent<StateMachineBattle>().selectPCTarget(target);
	}

	public void showSelector()
	{
		target.transform.Find("Selector").gameObject.SetActive(true);
	}

	public void hideSelector()
	{
		target.transform.Find("Selector").gameObject.SetActive(false);
	}
}
