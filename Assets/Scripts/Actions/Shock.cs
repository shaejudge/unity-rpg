using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : BaseAction
{

	public Shock()
	{
		actionName = "Shock";
		actionDesc = "A weak flash of electricity.\n" +
					 "Useful on good conductors. Water conducts well.";
		basePower = 20;
		actionCost = 2;
		actionType = element.ELECTRIC;
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
