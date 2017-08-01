using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseClass : MonoBehaviour {

	public string charName;

	public int level;
	public int experience;

	public int maxHP;
	public int currentHP;

	public int maxMP;
	public int currentMP;

	public int strength;
	public int magic;
	public int defense;
	public int magdef;
	public int agility;
	public int luck;
	//luck is currently unused in all calculations, just a joke stat
	//very realistic calculations here, so luck never helps

	public BaseAction.element weakness;

	public List<BaseAction> skillList = new List<BaseAction>();

	public IEnumerator attackColorChanger(BaseAction.element type)
	{
		if (type == BaseAction.element.NEUTRAL)
		{
			deadColorChanger();
			yield break;
		}

		SpriteRenderer currentSprite = this.transform.GetComponent<SpriteRenderer>();
		switch (type)
		{
			case (BaseAction.element.FIRE):
				currentSprite.color = new Color32(0xFF, 0x00, 0x00, 0xFF);
				break;

			case (BaseAction.element.WATER):
				currentSprite.color = new Color32(0x00, 0x00, 0xFF, 0xFF);
				break;

			case (BaseAction.element.ELECTRIC):
				currentSprite.color = new Color32(0xFF, 0xFF, 0x00, 0xFF);
				break;

			case (BaseAction.element.HEAL):
				currentSprite.color = new Color32(0x00, 0xFF, 0x00, 0xFF);
				break;

			default:
				//if not flashing, just do nothing until we implement something for that element
				break;
		}
		yield return new WaitForSeconds(0.0625f);

		deadColorChanger();
	}

	public void deadColorChanger()
	{
		SpriteRenderer currentSprite = this.transform.GetComponent<SpriteRenderer>();

		if (currentHP <= 0)
		{
			currentSprite.color = new Color32(0x80, 0x80, 0x80, 0xFF);
		}

		else
		{
			currentSprite.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
		}
	}
}
