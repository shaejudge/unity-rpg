using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : BaseAction
{

	public Heal()
	{
		actionName = "Heal";
		actionDesc = "A light healing glow.";
		basePower = 10;
		actionCost = 1;
		actionType = element.HEAL;
	}

	public override int calculate(BaseClass actor, BaseClass target)
	{
		//actor.currentMP -= actionCost;

		float healMultiplier = 1f + (float)basePower / 100f;

		//some games have MIND stat affect healing amount (INT affecting magic damage)
		//MIND is essentially magic defense for our purposes, so we take the target's magdef
		//higher means more healing
		float healAmount = (actor.magic + target.magdef) * 2 * healMultiplier;
		healAmount *= Random.Range(0.9f, 1.1f);

		int intHealAmount = (int)healAmount;

		//if weak to HEAL type, probably undead
		//we won't have undead in our game, but still
		//so if weak to heal, healing amount becomes damage amount
		//possibility of huge damage, since greater magic defense causes greater damage from healing
		if (target.weakness == actionType)
		{
			intHealAmount = -intHealAmount;
		}

		return intHealAmount;
	}
}
