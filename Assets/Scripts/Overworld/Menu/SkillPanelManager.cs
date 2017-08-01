using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelManager : MonoBehaviour {

	public GameObject skillPanelTemplate;
	public GameObject skillButtonTemplate;
	public GameObject targetButtonTemplate;
	public GameObject cancelButtonTemplate;

	public GameObject skillDescPanel;
	public Text skillDescText;

	public GameObject targetPanel;
	public Transform targetInnerPanel;

	public Transform innerPanel;

	public bool isActive = false;

	//not really the best name outside of battle, but eh
	public SkillHandler playerChoice;

	// Use this for initialization
	void Start () {
		deactivateSkillDescPanel();
		deactivateTargetPanel();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void toggle()
	{
		if (isActive)
		{
			deactivateSkillPanel();
		}
		else
		{
			activateSkillPanel();
		}
	}

	public void activateSkillPanel()
	{
		createPartySkills();
		createTargetButtons();
		this.gameObject.SetActive(true);
		playerChoice = new SkillHandler(); //this is done here so we have a fresh skillHandler when opening
		isActive = true;
	}

	public void deactivateSkillPanel()
	{
		deactivateTargetPanel();
		this.gameObject.SetActive(false);
		destroyPartySkills();
		destroyTargetButtons();
		isActive = false;
	}

	public void activateSkillDescPanel(string message)
	{
		skillDescText.text = message;
		skillDescPanel.SetActive(true);
	}

	public void deactivateSkillDescPanel()
	{
		skillDescPanel.SetActive(false);
	}

	public void createPartySkills()
	{
		Debug.Log("creating skill listing");

		innerPanel = this.transform.GetChild(0);

		Transform partyHolder = GameObject.Find("PartyStats").transform;
		int partyCount = partyHolder.childCount;


		for (int i = 0; i < partyCount; i++)
		{
			GameObject newSkillPanel = Instantiate(skillPanelTemplate) as GameObject;
			BasePC currentPC = partyHolder.GetChild(i).GetComponent<BasePC>();

			StatsPanelData minimalSkillData = newSkillPanel.GetComponent<StatsPanelData>();

			minimalSkillData.charName.text = currentPC.charName;
			minimalSkillData.MP.text = currentPC.currentMP.ToString() + "/" + currentPC.maxMP.ToString();
			minimalSkillData.gmStats = currentPC;

			for (int j = 0; j < currentPC.skillList.Count; j++)
			{
				GameObject newSkillButton = Instantiate(skillButtonTemplate) as GameObject;
				BaseAction currentSkill = currentPC.skillList[j];
				newSkillButton.transform.GetComponent<OverworldSkillButton>().skillToPerform = currentSkill;
				newSkillButton.transform.GetComponent<OverworldSkillButton>().actor = currentPC;
				newSkillButton.transform.Find("Text").GetComponent<Text>().text = currentSkill.actionName;

				//only allow party healing on overworld,
				//this can be modified later to be more generic and allowing town warp skills and such
				if (currentSkill.actionCost > currentPC.currentMP || currentSkill.actionType != BaseAction.element.HEAL)
				{
					newSkillButton.GetComponent<Button>().interactable = false;
				}

				newSkillButton.transform.SetParent(newSkillPanel.transform.Find("GridLayout"));
			}

			newSkillPanel.transform.SetParent(innerPanel);
		}
	}

	//for now, didn't change the text of this function since it should probably work as is when copied over from StatsPanelManager
	public void destroyPartySkills()
	{
		innerPanel = this.transform.GetChild(0);

		int statsPanelCount = innerPanel.childCount;

		if (statsPanelCount > 0)
		{
			for (int i = statsPanelCount - 1; i >= 0; i--)
			{
				Transform currentChildTransform = innerPanel.GetChild(i);
				Destroy(currentChildTransform.gameObject);
			}
		}
	}

	public void createTargetButtons()
	{
		Debug.Log("creating target (only PCs) listing");

		targetInnerPanel = targetPanel.transform.GetChild(0);

		Transform partyHolder = GameObject.Find("PartyStats").transform;
		int partyCount = partyHolder.childCount;

		for (int i = 0; i < partyCount; i++)
		{
			GameObject newTargetButton = Instantiate(targetButtonTemplate) as GameObject;
			BasePC currentPC = partyHolder.GetChild(i).GetComponent<BasePC>();

			newTargetButton.GetComponent<OverworldTargetButton>().target = currentPC;
			newTargetButton.transform.GetChild(0).GetComponent<Text>().text = currentPC.charName;

			newTargetButton.transform.SetParent(targetInnerPanel);
		}

		GameObject newCancelButton = Instantiate(cancelButtonTemplate) as GameObject;
		newCancelButton.transform.SetParent(targetInnerPanel);
	}

	public void destroyTargetButtons()
	{
		targetInnerPanel = targetPanel.transform.GetChild(0);

		int targetPanelCount = targetInnerPanel.childCount;

		if (targetPanelCount > 0)
		{
			for (int i = targetPanelCount - 1; i >= 0; i--)
			{
				Transform currentChildTransform = targetInnerPanel.GetChild(i);
				Destroy(currentChildTransform.gameObject);
			}
		}
	}

	public void activateTargetPanel()
	{
		targetPanel.SetActive(true);
	}

	public void deactivateTargetPanel()
	{
		targetPanel.SetActive(false);
		//when deactivating target, also clear the skill handler
		playerChoice = new SkillHandler();
	}

	public void selectSkill(BasePC actor, BaseAction skill)
	{
		playerChoice.actorBasePC = actor;
		playerChoice.action = skill;
		activateTargetPanel();
	}

	public void selectTarget(BasePC target)
	{
		playerChoice.targetBasePC = target;
		executeSkill(playerChoice);
	}

	public void executeSkill(SkillHandler currentAction)
	{
		int adjustment = currentAction.action.calculate(currentAction.actorBasePC, currentAction.targetBasePC);

		currentAction.targetBasePC.currentHP += adjustment;

		if (currentAction.targetBasePC.currentHP > currentAction.targetBasePC.maxHP)
		{
			currentAction.targetBasePC.currentHP = currentAction.targetBasePC.maxHP;
		}

		for (int i = 0; i < innerPanel.childCount; i++)
		{
			StatsPanelData currentDisplayData = innerPanel.GetChild(i).GetComponent<StatsPanelData>();
			BasePC currentPC = currentDisplayData.gmStats;
			currentDisplayData.updateMP();
			Transform currentGrid = innerPanel.GetChild(i).Find("GridLayout");

			for (int j = 0; j < currentGrid.childCount; j++)
			{
				Transform currentSkillButton = currentGrid.GetChild(j);
				BaseAction currentSkill = currentSkillButton.GetComponent<OverworldSkillButton>().skillToPerform;
				if (currentPC.currentMP < currentSkill.actionCost)
				{
					currentSkillButton.GetComponent<Button>().interactable = false;
				}
			}
		}
		GameManager.instance.mainMenu.statusPanelManager.fullUpdate();

		deactivateTargetPanel();
	}
}
