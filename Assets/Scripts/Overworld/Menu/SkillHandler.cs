using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SkillHandler {

	//don't need target types, because everything's a PC when on the overworld
	public BasePC actorBasePC;
	public BasePC targetBasePC;

	public BaseAction action;

	public bool aoe;
	//unused for now, a bool to say that an attack was over the entire party
	//also possible for over entire enemy party
}
