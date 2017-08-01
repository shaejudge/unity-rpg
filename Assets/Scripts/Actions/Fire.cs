using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : BaseAction {

	public Fire()
	{
		actionName = "Flare"; //Not named "Flare" in files because Unity seems to have special meaning for "Flare".
		actionDesc = "A small burst of fire.\n" + 
			         "Good against woody plants.\n" +
					 "Probably other carbon-based lifeforms as well.";
		basePower = 20;
		actionCost = 2;
		actionType = element.FIRE;
	}

	public override int calculate(BaseClass actor, BaseClass target)
	{
		//actor.currentMP -= actionCost;

		float defMultiplier = 1f;
		if (target.weakness == actionType)
		{
			defMultiplier *= 0.5f;
		}

		float atkMultiplier = 1f + (float)basePower / 100f;
		float damage = atkMultiplier * (actor.magic + basePower) - defMultiplier * target.magdef;

		if (target.weakness == actionType)
		{
			damage *= Random.Range(1.5f, 2f);
		}
		else
		{
			damage *= Random.Range(0.9f, 1.1f);
		}

		int intDamage = Mathf.Max((int)damage, 1);

		return -intDamage;
	}
}
