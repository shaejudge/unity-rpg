using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldTargetButton : MonoBehaviour {

	public BasePC target;

	public void selectPCTarget()
	{
		GameManager.instance.mainMenu.skillPanelManager.selectTarget(target);
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
