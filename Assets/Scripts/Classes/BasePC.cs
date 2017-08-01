using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasePC : BaseClass {

	//Can be slash or bash. Or shoot, but I'd assume arrows/bullets count as slicing damage
	public BaseAction baseAttack;
}
