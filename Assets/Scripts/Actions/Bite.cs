using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bite : BaseAction {

	public Bite()
	{
		actionName = "Bite";
		actionDesc = "A biting attack. Like mouth slashing.";
		basePower = 10;
		actionCost = 0;
		actionType = element.SHARP;
	}

	public override int calculate(BaseClass actor, BaseClass target)
	{
		float defMultiplier = 1f;
		if (target.weakness == actionType)
		{
			defMultiplier *= 0.5f;
		}

		float atkMultiplier = 1f + (float)basePower / 100f;
		float damage = atkMultiplier * (actor.strength + basePower) - defMultiplier * target.defense;

		if (target.weakness == actionType)
		{
			//Slash also gets a slight bonus in possibly doing up to triple rather than double damage when effective
			//"Bite" is given a slightly different range than regular slashing. Untested for balance.
			damage *= Random.Range(2f, 2.5f);
		}
		else
		{
			//However, Slash has a penalty of possibly doing down to half damage when not effective
			//Think of it as the sharp object not catching the enemy's skin/clothing just right
			//It can also do up to 1.5 damage if lucky
			damage *= Random.Range(0.5f, 1.5f);
		}

		int intDamage = Mathf.Max((int)damage, 1);

		return -intDamage;
	}
}
