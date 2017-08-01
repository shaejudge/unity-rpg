using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateMachineBattle : MonoBehaviour {

	public bool specialBattle = false;
	public string postBattleScene = "";
	public bool musicATB = false;

	public enum battleAction {WAIT, START, PERFORM, CHECK, WIN, LOSE};

	public battleAction battleState;

	//Trying something new here with two bools
	public bool animationInProgress = false;
	public bool menuInProgress = false;

	public List<TurnHandler> performList = new List<TurnHandler>();
	public List<GameObject> pcList = new List<GameObject>();
	public List<GameObject> enemyList = new List<GameObject>();
	public int experiencePot = 0;

	public enum playerGUI {ACTIVATE, WAITING, ACTION, TARGET, DONE};

	public playerGUI inputState;

	public List<GameObject> manageList = new List<GameObject>();
	private TurnHandler playerChoice;

	//Dynamic creation of target select
	public GameObject baseTargetButton;
	public GameObject targetPanel;
	public Transform targetSpacer;

	//Dynamic creation of alternative target
	public GameObject basePCTargetButton;
	public GameObject PCTargetPanel;
	public Transform PCTargetSpacer;

	//Dynamic creation of main battle menu
	public GameObject baseActionButton;
	public GameObject actionPanel;
	public Transform actionSpacer;

	//Dynamic creation of skill menu, if applicable (characters could have no skills)
	//Our game only has one character, so we're unlikely to ever encounter the disabled skill menu
	public GameObject baseSkillButton;
	public GameObject skillPanel;
	public Transform skillSpacer;

	public GameObject baseCancelButton;

	//trying something new instead of just console prints
	public GameObject messagePanel;
	public Text messageText;
	private float messageTimer;
	private float messageTTL;

	public GameObject skillDescPanel;
	public Text skillDescText;

	private List<GameObject> actionButtonList = new List<GameObject>();

	public List<Transform> enemySpawnPoints = new List<Transform>();

	//an idea for something similar to enemySpawnPoints, but not used for now
	//public List<Vector2> PCLocations = new List<Vector2>();

	private void Awake()
	{
		//the first part of this "if" is because special battles don't have enemy spawn data (they're "premade")
		//the second part of this "if" is just for when testing battles outside of world context, because the GameManager wouldn't exist
		if (!specialBattle && GameManager.instance)
		{
			//first handle enemy setup
			enemySetup();
		}
		if (GameManager.instance)
		{
			//next is PC setup, gathered from the GameManager
			pcSetup();
		}
	}

	private void enemySetup()
	{
		for (int i = 0; i < GameManager.instance.enemyAmount; i++)
		{
			GameObject newEnemy = Instantiate(GameManager.instance.enemiesInBattle[i], enemySpawnPoints[i].position, Quaternion.identity) as GameObject;
			collectEnemy(newEnemy);
		}
		//not used to lambdas in C#, but this should work based on some quick googling
		enemyList.Sort((a, b) => -a.transform.position.y.CompareTo(b.transform.position.y));
		for (int i = 0; i < GameManager.instance.enemyAmount; i++)
		{
			GameObject currentEnemy = enemyList[i];
			//first append index to name for uniqueness
			currentEnemy.name = currentEnemy.GetComponent<StateMachineEnemy>().charObject.charName + (i + 1).ToString();
			//this is a minor thing, but I think it should help with layering enemies that overlap
			currentEnemy.GetComponent<Renderer>().sortingOrder = i;
			//then set that unique name back to the character
			currentEnemy.GetComponent<StateMachineEnemy>().charObject.charName = currentEnemy.name;
			experiencePot += currentEnemy.GetComponent<StateMachineEnemy>().charObject.experience;
		}
	}

	private void pcSetup()
	{
		Transform party = GameManager.instance.transform.Find("PartyStats");
		int partyCount = party.childCount;

		Transform pcObjectHolder = GameObject.Find("PCHolder").transform;
		//Note that PCHolder should have the same number of children as the GM party
		//if not, everything will crash and burn

		for (int i = 0; i < partyCount; i++)
		{
			Transform currentMember = party.GetChild(i);
			BasePC gmStats = currentMember.GetComponent<BasePC>();
			for (int j = 0; j < partyCount; j++)
			{
				Transform currentBattlePC = pcObjectHolder.GetChild(j);
				BasePC battleStats = currentBattlePC.GetComponent<BasePC>();

				//a simple copy of all stats
				//some will end up by reference, but we aren't altering anything within battle
				//aside from currentHP and currentMP, which are just ints
				if (gmStats.charName == battleStats.charName)
				{
					battleStats.maxHP = gmStats.maxHP;
					battleStats.currentHP = gmStats.currentHP;
					battleStats.maxMP = gmStats.maxMP;
					battleStats.currentMP = gmStats.currentMP;
					battleStats.strength = gmStats.strength;
					battleStats.magic = gmStats.magic;
					battleStats.defense = gmStats.defense;
					battleStats.magdef = gmStats.magdef;
					battleStats.agility = gmStats.agility;
					battleStats.luck = gmStats.luck;
					battleStats.weakness = gmStats.weakness;
					battleStats.skillList = gmStats.skillList;
					battleStats.baseAttack = gmStats.baseAttack;
					break;
				}
			}
		}

		//Like before, we're sorting by Y position
		//not sure if this will really help much though
		pcList.Sort((a, b) => -a.transform.position.y.CompareTo(b.transform.position.y));
	}

	// Use this for initialization
	void Start ()
	{
		//this next part should be in Start, but moved here for out of context testing
		messagePanel.SetActive(false);
		//if a special message is noted, present it, then reset it in the GM
		if (GameManager.instance != null && GameManager.instance.battleStartMessage != "")
		{
			activateMessagePanel(GameManager.instance.battleStartMessage, 5f);
			GameManager.instance.battleStartMessage = "";
		}

		skillDescPanel.SetActive(false);
		//messageText = messagePanel.transform.FindChild("Text").GetComponent<Text>();
		//activateMessagePanel("Battle started.");
		battleState = battleAction.WAIT;
		inputState = playerGUI.ACTIVATE;

		actionPanel.SetActive(false);
		skillPanel.SetActive(false);
		targetPanel.SetActive(false);
		PCTargetPanel.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		updateMessagePanel();

		//more accurate musicATB implementation, but less pleasant
		//less active version handled in entering/leaving action panel helpers
		/*
		if (musicATB)
		{
			AudioSource audio = GameObject.Find("Music").GetComponent<AudioSource>();
			if (!timeActive())
			{
				audio.Pause();
			}
			else
			{
				audio.UnPause();
			}
		}
		*/

		switch (battleState)
		{
			case (battleAction.WAIT):
				if (performList.Count > 0)
				{
					battleState = battleAction.START;
				}
				break;

			case (battleAction.START):
				startAction();
				battleState = battleAction.PERFORM;
				break;

			case (battleAction.PERFORM):
				//actor alerts BSM when done with action to reach next state
				break;

			case (battleAction.CHECK):
				if (pcList.Count < 1)
				{
					inputState = playerGUI.WAITING;
					clearPanels();
					battleState = battleAction.LOSE;
				}
				else if (enemyList.Count < 1)
				{
					inputState = playerGUI.WAITING;
					clearPanels();
					battleState = battleAction.WIN;
				}
				else
				{
					performList.RemoveAt(0);
					animationInProgress = false;
					battleState = battleAction.WAIT;
				}
				break;

			case (battleAction.LOSE):
				activateMessagePanel("You lost.", -1f);
				haltAll();
				StartCoroutine(postBattleLose());
				break;

			case (battleAction.WIN):
				activateMessagePanel("You won.", -1f);
				haltAll();
				StartCoroutine(postBattleWin() );
				break;
		}

		switch (inputState)
		{
			case (playerGUI.ACTIVATE):
				if (manageList.Count > 0)
				{
					manageList[0].GetComponent<StateMachinePC>().activateSelector();
					manageList[0].GetComponent<StateMachinePC>().isDefending = false;
					playerChoice = new TurnHandler();

					createActionButtons();
					createTargetButtons();
					createPCTargetButtons();

					actionPanel.SetActive(true);
					inputState = playerGUI.WAITING;
				}
				break;

			case (playerGUI.WAITING):
				//When menu is up, or when battle is over, GUI is left on WAITING state
				//Individual command/action functions will change the state to DONE as necessary
				break;

			case (playerGUI.DONE):
				finishInput();
				inputState = playerGUI.ACTIVATE;
				break;
		}
	}

	//a method that returns whether time should run
	public bool timeActive()
	{
		//time is only active if we are both NOT in menu (past initial menu) and NOT in animation
		if (!menuInProgress && !animationInProgress)
		{
			return true;
		}
		return false;
	}

	//called by enemies to automatically submit action to BSM
	public void collectAction(TurnHandler newAction)
	{
		performList.Add(newAction);
	}

	public void collectEnemy(GameObject enemy)
	{
		enemyList.Add(enemy);
	}

	public void collectPC(GameObject pc)
	{
		pcList.Add(pc);
	}

	//A separate function might be created later for targetting other PCs
	void createTargetButtons()
	{
		for (int i = 0; i < enemyList.Count; i++)
		{
			GameObject enemy = enemyList[i];

			GameObject newButton = Instantiate(baseTargetButton) as GameObject;
			TargetButton button = newButton.GetComponent<TargetButton>();

			StateMachineEnemy currentESM = enemy.GetComponent<StateMachineEnemy>();

			Text label = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
			label.text = currentESM.charObject.charName;

			button.target = enemy;

			newButton.transform.SetParent(targetSpacer, false);

			actionButtonList.Add(newButton);
		}
		//Cancel button created outside of loop
		GameObject newCancelButton = Instantiate(baseCancelButton) as GameObject;
		newCancelButton.transform.SetParent(targetSpacer, false);
		actionButtonList.Add(newCancelButton);
	}

	//This is that separate function mentioned above
	void createPCTargetButtons()
	{
		for (int i = 0; i < pcList.Count; i++)
		{
			GameObject pc = pcList[i];

			GameObject newButton = Instantiate(basePCTargetButton) as GameObject;
			TargetButton button = newButton.GetComponent<TargetButton>();

			StateMachinePC currentPCSM = pc.GetComponent<StateMachinePC>();

			Text label = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
			label.text = currentPCSM.charObject.charName;

			button.target = pc;

			newButton.transform.SetParent(PCTargetSpacer, false);

			actionButtonList.Add(newButton);
		}
		//Cancel button created outside of loop
		GameObject newCancelButton = Instantiate(baseCancelButton) as GameObject;
		newCancelButton.transform.SetParent(PCTargetSpacer, false);
		actionButtonList.Add(newCancelButton);
	}

	void createActionButtons()
	{
		//Creating attack button
		GameObject attackButton = Instantiate(baseActionButton) as GameObject;
		Text attackText = attackButton.transform.Find("Text").gameObject.GetComponent<Text>();
		attackText.text = "Attack";
		attackButton.GetComponent<Button>().onClick.AddListener(() => attackSelect());
		attackButton.transform.SetParent(actionSpacer, false);

		actionButtonList.Add(attackButton);

		//Creating skill button
		GameObject skillButton = Instantiate(baseActionButton) as GameObject;
		Text skillText = skillButton.transform.Find("Text").gameObject.GetComponent<Text>();
		skillText.text = "Skills";
		skillButton.GetComponent<Button>().onClick.AddListener(() => skillSelect());
		skillButton.transform.SetParent(actionSpacer, false);

		actionButtonList.Add(skillButton);

		//only create individual skill buttons if current PC has skills to use
		if (manageList[0].GetComponent<StateMachinePC>().charObject.skillList.Count > 0)
		{
			List<BaseAction> skillList = manageList[0].GetComponent<StateMachinePC>().charObject.skillList;
			for (int i = 0; i < skillList.Count; i++)
			{
				BaseAction skill = skillList[i];

				GameObject newSkillButton = Instantiate(baseSkillButton) as GameObject;
				Text skillButtonText = newSkillButton.transform.Find("Text").gameObject.GetComponent<Text>();
				skillButtonText.text = skill.actionName;
				SkillButton skillData = newSkillButton.GetComponent<SkillButton>();
				skillData.skillToPerform = skill;

				//if a skill's MP cost exceeds the PC's current MP, make it unselectable
				//this check isn't performed again elsewhere, so MP can go negative if this check is removed
				if (skill.actionCost > manageList[0].GetComponent<StateMachinePC>().charObject.currentMP)
				{
					newSkillButton.GetComponent<Button>().interactable = false;
				}

				newSkillButton.transform.SetParent(skillSpacer, false);
				actionButtonList.Add(newSkillButton);
			}
			//Cancel button created outside of loop
			GameObject newCancelButton = Instantiate(baseCancelButton) as GameObject;
			newCancelButton.transform.SetParent(skillSpacer, false);
			actionButtonList.Add(newCancelButton);
		}
		else
		{
			skillButton.GetComponent<Button>().interactable = false;
		}

		//a defense button
		//Creating attack button
		GameObject defendButton = Instantiate(baseActionButton) as GameObject;
		Text defendText = defendButton.transform.Find("Text").gameObject.GetComponent<Text>();
		defendText.text = "Defend";
		defendButton.GetComponent<Button>().onClick.AddListener(() => defendSelect());
		defendButton.transform.SetParent(actionSpacer, false);

		actionButtonList.Add(defendButton);
	}

	//starts initial 
	public void startAction()
	{
		GameObject currentActor = performList[0].actorGameObject;
		//bool found = false;
		if (performList[0].actorType == "Enemy")
		{
			StateMachineEnemy esm = currentActor.GetComponent<StateMachineEnemy>();
			esm.actionTarget = performList[0].targetGameObject;
			esm.currentState = StateMachineEnemy.turnState.ACTION;
		}

		else if (performList[0].actorType == "PC")
		{
			StateMachinePC pcsm = currentActor.GetComponent<StateMachinePC>();
			pcsm.actionTarget = performList[0].targetGameObject;
			pcsm.currentState = StateMachinePC.turnState.ACTION;
		}
	}

	//This is called when the "attack" button is selected
	public void attackSelect()
	{
		playerChoice.actorGameObject = manageList[0];
		playerChoice.actorType = "PC";
		//Characters can have unique base attacks
		playerChoice.action = manageList[0].GetComponent<StateMachinePC>().charObject.baseAttack;
		playerChoice.usingSkill = false;

		leavingActionPanel();
		targetPanel.SetActive(true);
	}

	//This is called when the "skills" button is selected
	public void skillSelect()
	{
		playerChoice.actorGameObject = manageList[0];
		playerChoice.actorType = "PC";

		leavingActionPanel();
		skillPanel.SetActive(true);
	}

	//This is called when the "defend" button is selected
	public void defendSelect()
	{
		StateMachinePC currentPC = manageList[0].GetComponent<StateMachinePC>();
		currentPC.startDefending();
		playerChoice.usingSkill = false;

		activateMessagePanel(currentPC.charObject.charName + " defends against incoming attacks.");

		//clearPanels();
		inputState = playerGUI.DONE;
	}

	//This is called when a specific skill is selected
	public void selectSkill(BaseAction skill)
	{
		playerChoice.action = skill;
		playerChoice.usingSkill = true;

		//skillPanel.SetActive(false);

		//if healing, use alternate PC target panel
		//the selfTarget flag tells the PC to stay put instead of walking around
		//it could be used for enemies too, but enemies currently only ever attack
		if (skill.actionType == BaseAction.element.HEAL)
		{
			playerChoice.selfTarget = true;
			PCTargetPanel.SetActive(true);
		}
		else
		{
			playerChoice.selfTarget = false;
			targetPanel.SetActive(true);
		}
	}

	//determines the target of attack/skill
	public void selectTarget(GameObject target)
	{
		playerChoice.targetGameObject = target;
		playerChoice.targetType = "Enemy";

		//Remember to get rid of the target's "arrow" after making choice
		/*
		target.transform.FindChild("Selector").gameObject.SetActive(false);
		*/
		//Moved out to an onClick event now

		//targetPanel.SetActive(false);
		//clearPanels();
		performList.Add(playerChoice);

		inputState = playerGUI.DONE;
	}

	//like selectTarget, but for healing skills directed towards player's party
	public void selectPCTarget(GameObject target)
	{
		playerChoice.targetGameObject = target;
		playerChoice.targetType = "PC";
		//clearPanels();
		performList.Add(playerChoice);

		inputState = playerGUI.DONE;
	}

	//performs the last few actions after player finishes inputting command
	public void finishInput()
	{
		//Disable selector
		manageList[0].GetComponent<StateMachinePC>().deactivateSelector();
		//Clear out menus, rebuilt later per PC as appropriate
		clearPanels();
		//Remove head of management list
		manageList.RemoveAt(0);

		if (musicATB)
		{
			AudioSource audio = GameObject.Find("Music").GetComponent<AudioSource>();
			audio.volume = 0.75f;
		}
	}

	//leaving and entering the main action panel toggles whether time should be running
	public void leavingActionPanel()
	{
		//actionPanel.SetActive(false);
		//timeActive set to false only after first menu to accurately mimic wait ATB
		//this is now handled by a separate method
		menuInProgress = true;

		if (musicATB)
		{
			AudioSource audio = GameObject.Find("Music").GetComponent<AudioSource>();
			audio.volume = 0.25f;
		}
	}

	public void enteringActionPanel()
	{
		//actionPanel.SetActive(true);
		//timeActive set to true again on main battle menu, again for accuracy to wait ATB
		//now handled by separate method
		menuInProgress = false;

		if (musicATB)
		{
			AudioSource audio = GameObject.Find("Music").GetComponent<AudioSource>();
			audio.volume = 0.75f;
		}
	}

	//This clears all menu panels, but not the message panel up top
	//the skill description is considered a menu element, even though it's not selectable
	public void clearPanels()
	{
		menuInProgress = false;
		actionPanel.SetActive(false);
		skillPanel.SetActive(false);
		targetPanel.SetActive(false);
		PCTargetPanel.SetActive(false);
		skillDescPanel.SetActive(false);
		foreach (GameObject button in actionButtonList)
		{
			Destroy(button);
		}
		actionButtonList.Clear();
	}

	//A single function to handle all cancellations
	//Maybe not the best way to handle cancellation from different menus
	public void cancelMenu()
	{
		//if target panel is active
		if (targetPanel.activeSelf)
		{
			//deactivate it
			targetPanel.SetActive(false);
			//then check if skill panel is still active behind it
			if (!skillPanel.activeSelf)
			{
				//if not, then the time should start running again
				enteringActionPanel();
			}
			//(if skill panel active, time stays paused)
		}
		//if PC target panel is active
		else if (PCTargetPanel.activeSelf)
		{
			//deactivate it, and nothing else needs to be done since PCs can only be targetted by skills
			PCTargetPanel.SetActive(false);
		}
		//if none of the above applies, we must be in skill panel, so we just deactivate it and start the time running again
		else
		{
			skillPanel.SetActive(false);
			enteringActionPanel();
		}
	}

	//The tiny skill description is treated slightly differently from other panels
	//Works mostly on mouse-over
	public void activateSkillDescPanel(string message)
	{
		skillDescText.text = message;
		skillDescPanel.SetActive(true);
	}

	public void deactivateSkillDescPanel()
	{
		skillDescPanel.SetActive(false);
	}

	//activation requires a string to display, default display time is 3 seconds
	//newer messages simply override existing messages
	//so it's not quite like a console
	//to make it more console-like, could add expiration time to specific messages rather than the message window
	public void activateMessagePanel(string message, float TTL = 3f)
	{
		messageText.text = message;
		messagePanel.SetActive(true);
		messageTTL = TTL;
		messageTimer = 0f;
	}

	//updates the timer for the message, and shuts it off when appropriate
	public void updateMessagePanel()
	{
		//Setting messageTTL to a negative float means never off, essentially
		//probably should note this elsewhere
		if (messagePanel.activeSelf && messageTTL > 0)
		{
			messageTimer += Time.deltaTime;
			if (messageTimer >= messageTTL)
			{
				messagePanel.SetActive(false);
				messageText.text = "";
			}
		}
	}

	//this tells all non-dead characters to stop everything
	//dead characters stay dead, so their state isn't changed
	public void haltAll()
	{
		for (int i = 0; i < pcList.Count; i++)
		{
			StateMachinePC pcsm = pcList[i].GetComponent<StateMachinePC>();
			if (pcsm.currentState != StateMachinePC.turnState.DEAD)
			{
				pcsm.currentState = StateMachinePC.turnState.WAITING;
			}
		}
		for (int i = 0; i < enemyList.Count; i++)
		{
			StateMachineEnemy esm = enemyList[i].GetComponent<StateMachineEnemy>();
			if (esm.currentState != StateMachineEnemy.turnState.DEAD)
			{
				esm.currentState = StateMachineEnemy.turnState.WAITING;
			}
		}
		performList.Clear();
	}

	//this only exists because I'm not sure the WaitForSeconds class can be used outside of coroutines
	private IEnumerator postBattleWin()
	{
		yield return new WaitForSeconds(3f);

		//if special, we have new scene after battle ends, not old scene
		//boss battles presumably don't lead back to the exact same place
		if (specialBattle)
		{
			GameManager.instance.previousScene = postBattleScene;
			GameManager.instance.nextCharPosition = Vector2.zero;
			//nextCharPosition is zeroed to indicate it's no longer relevant
			//(to be determined by next scene)

			//note that gameStatus could be left on IDLE, not changed to OVERWORLD
			//idea would be cutscenes are still "idle", not allowing menu or battles
			//current method is to set overworld, but have a specific script
			//in cutscenes that forces it back to idle until player is allowed to
			//act freely again
		}
		/*
		//This else is for the opposite OVERWORLD/IDLE decision mentioned above
		else
		{
			GameManager.instance.gameState = GameManager.gameStatus.OVERWORLD;
		}
		*/
		GameManager.instance.gameState = GameManager.gameStatus.OVERWORLD;
		pcStatsWriteback();
		GameManager.instance.enemiesInBattle.Clear();
		GameManager.instance.loadSceneAfterBattle();
	}

	//like the pcSetup function, but in reverse and covering less data
	private void pcStatsWriteback()
	{
		Transform party = GameManager.instance.transform.Find("PartyStats");
		int partyCount = party.childCount;

		Transform pcObjectHolder = GameObject.Find("PCHolder").transform;
		//Note that PCHolder should have the same number of children as the GM party
		//if not, everything will crash and burn

		for (int i = 0; i < partyCount; i++)
		{
			Transform currentMember = party.GetChild(i);
			BasePC gmStats = currentMember.GetComponent<BasePC>();
			for (int j = 0; j < partyCount; j++)
			{
				Transform currentBattlePC = pcObjectHolder.GetChild(j);
				BasePC battleStats = currentBattlePC.GetComponent<BasePC>();

				//a simple copy of all stats
				//some will end up by reference, but we aren't altering anything within battle
				//aside from currentHP and currentMP, which are just ints
				if (gmStats.charName == battleStats.charName)
				{

					gmStats.currentHP = battleStats.currentHP;
					//gmStats.currentMP = battleStats.currentMP;
					gmStats.currentMP = gmStats.maxMP;
					//giving MP refill after battle for now, because inventory system doesn't support consumeable heal items
					gmStats.experience += experiencePot;

					//revive dead guys with 1 HP because we're nice
					if (gmStats.currentHP <= 0)
					{
						gmStats.currentHP = 1;
					}
					break;
				}
			}
		}
	}

	private IEnumerator postBattleLose()
	{
		yield return new WaitForSeconds(3f);

		GameManager.instance.loadStartMenu();
	}

	//while not an IEnumerator, applyDamage is moved to end for ease of access here
	//applyDamage seems to be what might require the most overhaul as the game's development progresses
	public void applyDamage()
	{
		bool defending = false;
		TurnHandler currentHandler = performList[0];
		BaseClass currentActor;
		BaseClass currentTarget;
		BaseAction.element attackType = currentHandler.action.actionType;

		if (currentHandler.actorType == "PC")
		{
			currentActor = currentHandler.actorGameObject.GetComponent<StateMachinePC>().charObject;
		}
		else
		{
			currentActor = currentHandler.actorGameObject.GetComponent<StateMachineEnemy>().charObject;
		}

		if (currentHandler.usingSkill)
		{
			currentActor.currentMP -= currentHandler.action.actionCost;
		}

		if (currentHandler.targetType == "PC")
		{
			currentTarget = currentHandler.targetGameObject.GetComponent<StateMachinePC>().charObject;
			if (currentHandler.targetGameObject.GetComponent<StateMachinePC>().isDefending)
			{
				defending = true;
			}
		}
		else
		{
			currentTarget = currentHandler.targetGameObject.GetComponent<StateMachineEnemy>().charObject;
			if (currentHandler.targetGameObject.GetComponent<StateMachineEnemy>().isDefending)
			{
				defending = true;
			}
		}

		int adjustment = currentHandler.action.calculate(currentActor, currentTarget);

		string currentMessage = currentActor.charName + " used " + currentHandler.action.actionName + " on "
							+ currentTarget.charName + ".\n";

		if (adjustment > 0)
		{
			currentMessage += "Healed for " + adjustment.ToString() + " health.";
		}
		else
		{
			if (defending)
			{
				adjustment = adjustment / 2;
				currentMessage += currentTarget.charName + " is defending. ";
			}
			currentMessage += (-adjustment).ToString() + " damage.";
		}

		if (currentHandler.targetType == "PC")
		{
			currentHandler.targetGameObject.GetComponent<StateMachinePC>().adjustHP(adjustment);
			StartCoroutine(currentHandler.targetGameObject.GetComponent<StateMachinePC>().charObject.attackColorChanger(attackType));
		}
		else
		{
			currentHandler.targetGameObject.GetComponent<StateMachineEnemy>().adjustHP(adjustment);
			StartCoroutine(currentHandler.targetGameObject.GetComponent<StateMachineEnemy>().charObject.attackColorChanger(attackType));
		};

		activateMessagePanel(currentMessage);
	}

	//this is currently unused
	/*
	public IEnumerator colorFlash(GameObject flasher, BaseAction.element element)
	{
		SpriteRenderer currentSprite = flasher.GetComponent<SpriteRenderer>();
		Color32 previousColor = currentSprite.color;
		switch (element)
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
				//if not flashing, just do nothing
				yield break;
				//break; //this is unreachable, but I think all switch cases should end with a break
		}
		yield return new WaitForSeconds(0.125f);

		currentSprite.color = previousColor;

	}
	*/
}
