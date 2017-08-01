using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseClass {

	public enum rarity {COMMON, UNCOMMON, RARE};
	public rarity enemyRarity;
}
