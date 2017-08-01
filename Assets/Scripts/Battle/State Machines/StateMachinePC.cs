using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateMachinePC : MonoBehaviour {

	private StateMachineBattle bsm;
	public BasePC charObject;

	public enum turnState {CHARGING, ADDLIST, WAITING, CHOICE, ACTION, DEAD};
	public turnState currentState;

	public bool isDefending = false;

	//10s because some people have told me that 5s timer seems too fast
	//down to 6s again because 10s feels slow to me
	public float currentATB = 0f;
	public float maxATB = 6f;
	private float animSpeed = 10f;

	//keeping track of initial position
	private Vector2 startPosition;

	public Image atbBar;
	public GameObject selector;

	private bool alive = true;

	private PlayerPanelData panelData;
	public GameObject pcPanel;
	private Transform rightPanelSpacer;

	public GameObject actionTarget;
	private bool actionStarted = false;

	//need to add to BSM before Start
	private void Awake()
	{
		charObject = this.gameObject.GetComponent<BasePC>();

		//list addition is done in Awake rather than Start for some timing reasons
		//idea is that pcList for BSM is built in Awake phase,
		//sorting and applying current stats is done by BSM in Start phase
	}

	// Use this for initialization (currently unused?)
	void Start ()
	{
		bsm = GameObject.Find("BattleManager").GetComponent<StateMachineBattle>();
		this.gameObject.transform.SetParent(GameObject.Find("PCHolder").transform);
		bsm.collectPC(this.gameObject);

		rightPanelSpacer = GameObject.Find("BattleCanvas").transform.Find("RightPanel").transform.Find("RightPanelSpacer");
		createPCPanel();

		agilityMath();
		currentState = turnState.CHARGING;
		selector.SetActive(false);
		startPosition = (Vector2)transform.position;
	}

	//had an idea for storing the actual gameObjects along with GameManager,
	//but now switched to simply storing and copying stats
	public void LateStart()
	{
	}

	void createPCPanel()
	{
		pcPanel = Instantiate(pcPanel) as GameObject;
		panelData = pcPanel.GetComponent<PlayerPanelData>();
		panelData.pcName.text = charObject.charName;
		atbBar = panelData.atbBar;
		updatePCPanel();
		pcPanel.transform.SetParent(rightPanelSpacer, false);
	}

	//A special function for agility bonuses.
	void agilityMath()
	{
		//First off, maxATB is lowered by 1/100 of agility.
		//A steady bonus, but not too strong. We shouldn't ever go over 100,
		//so the max bonus would be 4 second ATB bar at best. (Likely not even close.)
		//1/10 of agility becomes the base starting ATB bar status in each battle.
		//The standard up to 50% bar bonus is applied to all characters on top of that.
		//(these comments aren't entirely valid now because of even stronger agility bonuses to facilitate testing)
		float floatAgility = (float)charObject.agility;
		float deciFA = floatAgility / 10;
		float centiFA = deciFA / 10;
		maxATB -= deciFA;
		currentATB = Mathf.Clamp(centiFA + Random.Range(0f, maxATB / 2), 0f, maxATB);

		//Overall, 50 agility ensures first strike. (Though time is always running)
		//100 agility makes each turn happen 1 second faster. (Again, time is running even in menus)
		//500 agility is impossible, but would give instantaneous turns,
		//limited by attack animation and thinking speed. (also actual fps, of course)
	}
	
	// Update is called once per frame
	void Update ()
	{
		updatePCPanel();
		//I don't usually like switch statements, but switches are for state machines.
		switch (currentState)
		{
			case (turnState.CHARGING):
				advanceATB();
				break;

			case (turnState.ADDLIST):
				bsm.manageList.Add(this.gameObject);
				currentState = turnState.WAITING;
				break;

			case (turnState.WAITING):
				//BSM alerts PCSM when action is ready
				break;

			case (turnState.ACTION):
				StartCoroutine(actioning() );
				break;

			case (turnState.DEAD):
				if (alive)
				{
					deadCleanup();
				}
				//maintain dead color
				//charObject.deadColorChanger();
				break;
		}
	}

	void advanceATB()
	{
		if (bsm.timeActive() )
		{
			currentATB = currentATB + Time.deltaTime;
			float barFill = currentATB / maxATB;
			//Can't let bar overfill (might happen with weird processing errors?)
			//Also note that "pixel perfect" in Canvas settings breaks localScale transforms
			atbBar.transform.localScale = new Vector2(Mathf.Clamp(barFill, 0f, 1f), atbBar.transform.localScale.y);
			if (currentATB >= maxATB)
			{
				atbBar.color = new Color32(0x75, 0xBB, 0x75, 0xFF);
				currentState = turnState.ADDLIST;
			}
		}
	}

	//moved here for consistency reasons
	public void startDefending()
	{
		isDefending = true;
		currentATB = maxATB / 2;
		atbBar.color = new Color32(0xBB, 0xBB, 0x75, 0xFF);
		currentState = StateMachinePC.turnState.CHARGING;
	}

	//Adjusts HP by amount. Negative for damage, positive for healing.
	public void adjustHP(int adjustment)
	{
		charObject.currentHP += adjustment;
		if (charObject.currentHP <= 0)
		{
			charObject.currentHP = 0;
			currentState = turnState.DEAD;
		}
		else if (charObject.currentHP >= charObject.maxHP)
		{
			charObject.currentHP = charObject.maxHP;
		}
	}

	//the little status bar below is kept up to date in each update
	void updatePCPanel()
	{
		panelData.pcHP.text = "HP " + charObject.currentHP.ToString() + "/" + charObject.maxHP.ToString();
		if (charObject.currentHP < charObject.maxHP / 4)
		{
			panelData.pcHP.color = new Color32(0xFF, 0x44, 0x44, 0xFF);
		}
		else
		{
			panelData.pcHP.color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
		}
		panelData.pcMP.text = "MP " + charObject.currentMP.ToString() + "/" + charObject.maxMP.ToString();
	}

	//simple method to toggle selector
	//the name's color is also highlighted for clarity
	//this method just bundles the two tasks together
	public void activateSelector()
	{
		selector.SetActive(true);
		panelData.pcName.color = new Color32(0xff, 0xff, 0x00, 0xff);

	}

	public void deactivateSelector()
	{
		selector.SetActive(false);
		panelData.pcName.color = new Color32(0xff, 0xff, 0xff, 0xff);
	}

	public void deadCleanup()
	{
		//change tag
		this.gameObject.tag = "DeadPC";
		//remove as potential target for enemies (and for heals)
		bsm.pcList.Remove(this.gameObject);
		//if currently selected, do end-turn actions as necessary
		if (selector.activeSelf)
		{
			bsm.finishInput();
			bsm.inputState = StateMachineBattle.playerGUI.ACTIVATE;
		}
		else
		{
			//still attempt to remove from management list in BSM if not active
			bsm.manageList.Remove(this.gameObject);
		}
		//remove tasks from performList
		//iterate backwards to avoid index errors
		for (int i = bsm.performList.Count - 1; i >= 1; i--)
		{
			//if this dead character is actor, cancel the action 
			if (bsm.performList[i].actorGameObject == this.gameObject)
			{
				bsm.performList.RemoveAt(i);
			}
			//if this dead character is target, attempt random redirection
			else if (bsm.performList[i].targetGameObject = this.gameObject)
			{
				if (bsm.pcList.Count > 0)
				{
					bsm.performList[i].targetGameObject = bsm.pcList[Random.Range(0, bsm.pcList.Count)];
				}
			}
		}

		//like with enemy death, we need to remove dead players as healable targets
		//may need a new panel for revives at some point, but that's not going to be relevant
		//our game will be single PC game
		if (bsm.PCTargetSpacer.childCount > 1)
		{
			for (int i = bsm.PCTargetSpacer.childCount - 2; i >= 0; i--)
			{
				GameObject currentButton = bsm.PCTargetSpacer.GetChild(i).gameObject;
				GameObject currentPC = currentButton.transform.GetComponent<TargetButton>().target;

				if (currentPC == this.gameObject)
				{
					Destroy(currentButton);
					break;
				}
			}
		}

		//death animation/position
		//sprite coloring now handled by BSM (actually by BaseClass, but called through BSM)
		atbBar.color = new Color32(0x75, 0x75, 0x75, 0xFF);

		//This won't be seen unless PC is revived, but set bar to half full upon next revive
		//Hopefully prevents exploits from intentional deaths and revives to stock turns
		//Not exactly relevant for our game given we plan to have a single PC for story reasons
		currentATB = maxATB / 2;

		alive = false;

		bsm.messageText.text += "\n" + charObject.charName + " has fallen.";
	}

	//Target isn't necessarily an object, could just be a spot on map.
	private bool moveToTarget(Vector2 targetPosition)
	{
		transform.position = Vector2.MoveTowards(transform.position, targetPosition, animSpeed * Time.deltaTime);

		//True means "done moving", which we take to be within 0.00001 unity units of desired position.
		if (Vector2.Distance(transform.position, targetPosition) <= .00001)
		{
			return true;
		}
		//If not done moving, return false.
		return false;
	}

	//alternative version that cuts early
	//currently unused
	private bool moveToTargetShort(Vector2 targetPosition)
	{
		transform.position = Vector2.MoveTowards(transform.position, targetPosition, animSpeed * Time.deltaTime);

		//based entirely on x axis, so movement stops after an imaginary vertical line
		if (Mathf.Abs(transform.position.x - targetPosition.x) <= 5f)
		{
			return true;
		}
		//If not done moving, return false.
		return false;
	}

	//like in the ESM, IEnumerator for action is moved to bottom for easy access
	private IEnumerator actioning()
	{
		if (actionStarted)
		{
			//Not used to this type of statement.
			//Reading up about it seems to indicate that
			//"yield break" tells the enumerator to terminate.
			//Iterators (I believe enumerators are a type of iterator)
			//generally end with a yield and pause execution (continuing at same time later),
			//but "yield break" says that it's time to stop, like a function return.
			yield break;
		}

		actionStarted = true;
		//Trying something new, time no longer runs during animations
		//the BSM will resume time in the CHECK state if necessary
		bsm.animationInProgress = true;

		//move to target (maybe not necessary?)
		Vector2 targetPosition = new Vector2(actionTarget.transform.position.x + 4f, actionTarget.transform.position.y);

		//if targetting other PC, don't animate
		if (bsm.performList[0].selfTarget == true)
		{
			yield return new WaitForSeconds(0.125f);
			bsm.applyDamage(); //actually applying heal
			yield return new WaitForSeconds(0.125f);

		}

		else
		{
			//return null (wait) while moving (conditional is while not done moving)
			while (!moveToTarget(targetPosition))
			{
				yield return null;
			}

			//Pause for half a half a second to think about life
			yield return new WaitForSeconds(0.125f);

			//above and below here is commented out because player movement makes game feel slow
			//attack animation goes here at some point
			bsm.applyDamage();

			//The pause is split in two, so the damage occurs right in the middle of the pause
			yield return new WaitForSeconds(0.125f);

			//animations to return to start
			while (!moveToTarget(startPosition))
			{
				yield return null;
			}
		}

		//stop everything if battle is over
		if (bsm.battleState == StateMachineBattle.battleAction.WIN
		 || bsm.battleState == StateMachineBattle.battleAction.LOSE)
		{
			currentState = turnState.WAITING;
		}
		//if not over, go back to WAITING and do necessary resets
		else
		{
			//reset BSM to WAIT, but CHECK for now to simplify things
			bsm.battleState = StateMachineBattle.battleAction.CHECK;

			actionStarted = false;

			//reset PC to charging ATB bar
			currentATB = 0f;
			atbBar.transform.localScale = new Vector2(0f, atbBar.transform.localScale.y);
			atbBar.color = new Color32(0xBB, 0xBB, 0x75, 0xFF);
			currentState = turnState.CHARGING;
		}

	}
}
