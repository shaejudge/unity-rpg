using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TurnHandler {

	//public string actorName;
	public string actorType;
	public string targetType;
	public GameObject actorGameObject;
	public GameObject targetGameObject;
	public bool usingSkill = false;

	//for custom damage calculations determined by skills
	//not used yet
	//public int damage;

	//action performed
	public BaseAction action;
	public bool selfTarget;
	//selfTarget refers to PCs targetting other PCs in general
	//it's unlikely enemies will heal each other
	//even if they do, they don't use the targetting panel
	//enemy self-targetting can be done completely through scripting
	//no need for special UI elements that are triggered by this bool
	public bool aoe;
	//unused for now, a bool to say that an attack was over the entire party
	//also possible for over entire enemy party
}
