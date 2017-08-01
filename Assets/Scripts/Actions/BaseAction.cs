using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAction : MonoBehaviour {

	public enum element {SHARP, BLUNT, FIRE, WATER, ELECTRIC, NEUTRAL, HEAL};

	public string actionName;
	public string actionDesc;
	public int basePower;
	public int actionCost;
	public element actionType;

	public virtual int calculate(BaseClass actor, BaseClass target)
	{
		//Damage is applied to HP as negative adjustment
		int damage = Mathf.Max(actor.strength - target.defense, 1);

		//return negative value
		return -damage;
	}
}
