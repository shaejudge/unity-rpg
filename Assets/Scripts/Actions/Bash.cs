using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bash : BaseAction {

	public Bash()
	{
		actionName = "Bash";
		actionDesc = "A blunt attack.";
		basePower = 15;
		actionCost = 0;
		actionType = element.BLUNT;
	}

	public override int calculate(BaseClass actor, BaseClass target)
	{
		float defMultiplier = 1f;
		float atkMultiplier = 1f;

		//blunt damage slightly ignores defense
		defMultiplier *= (1f - (float)basePower / 100f);

		if (target.weakness == actionType)
		{
			defMultiplier *= 0.5f;
			//in addition to standard halved defense for weakness,
			//bash gets basePower boost to attack like Slash gets
			atkMultiplier *= (1f + (float)basePower / 100f);
		}


		float damage = atkMultiplier * (actor.strength + basePower) - defMultiplier * target.defense;

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
