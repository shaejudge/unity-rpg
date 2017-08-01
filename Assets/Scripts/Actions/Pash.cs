using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pash : BaseAction {

	public Pash()
	{
		actionName = "Pash";
		actionDesc = "A poorer blunt attack.";
		basePower = 15;
		actionCost = 0;
		actionType = element.BLUNT;
	}

	public override int calculate(BaseClass actor, BaseClass target)
	{
		//just like excalipur, only does 1 damage
		return -1;
	}
}
