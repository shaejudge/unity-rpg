using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EWGF : BaseAction
{

	public EWGF()
	{
		actionName = "Fastest Wind God Fist";
		actionDesc = "A strong fist of electricity.\n" +
					 "Frequently seen in the iron fist tournament.";
		basePower = 15;
		actionCost = 3;
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

		//EWGF uses strength or magic, because why not
		//strongest of strength or magic attacks weakest of defense or magdef
		float atkMultiplier = 1f + (float)basePower / 100f;
		float damage = atkMultiplier * (Mathf.Max(actor.magic, actor.strength) + basePower) - defMultiplier * Mathf.Min(target.defense, target.magdef);

		if (target.weakness == actionType)
		{
			damage *= Random.Range(1.5f, 3f);
		}
		else
		{
			damage *= Random.Range(0.9f, 1.1f);
		}

		int intDamage = Mathf.Max((int)damage, 1);

		return -intDamage;
	}
}
