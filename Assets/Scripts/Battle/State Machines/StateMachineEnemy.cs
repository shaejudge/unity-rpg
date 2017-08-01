using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateMachineEnemy : MonoBehaviour {

	private StateMachineBattle bsm;
	public BaseEnemy charObject;

	//battle states
	public enum turnState {CHARGING, SELECTACTION, WAITING, ACTION, DEAD};
	public turnState currentState;

	public bool isDefending = false;

	//for ATB
	//Enemies generally get higher maxATB, to keep things manageable for the player
	//Currently at 12s, because why not
	//adjusted player down to 6s, so enemies are going down to 9 (any higher and battles might be too easy)
	public float currentATB = 0f;
	public float maxATB = 9f;
	private float animSpeed = 10f; //Not for ATB, but actual animation speed (moving left/right)

	//keeping track of initial position
	private Vector2 startPosition;

	//for taking action
	private bool actionStarted = false;

	public GameObject selector;

	private bool alive = true;

	public GameObject actionTarget;

	//need to add to BSM before Start
	private void Awake()
	{
		bsm = GameObject.Find("BattleManager").GetComponent<StateMachineBattle>();
		this.gameObject.transform.SetParent(GameObject.Find("EnemyHolder").transform);
		charObject = this.gameObject.GetComponent<BaseEnemy>();
	}

	// Use this for initialization
	void Start()
	{
		selector = transform.Find("Selector").gameObject;

		//This initial check is useless for random battles, but can help when testing non-random battles
		//maybe it should be commented out later
		bool found = false;
		for (int i = 0; i < bsm.enemyList.Count; i++)
		{
			if (bsm.enemyList[i] == this.gameObject)
			{
				found = true;
				break;
			}
		}
		if (!found)
		{
			bsm.collectEnemy(this.gameObject);
		}
		currentState = turnState.CHARGING;
		selector.SetActive(false);
		//We could work with Vector3, but I'm maintaining 2D-ness.
		//Casting position to a Vector2 for storage.
		startPosition = (Vector2)transform.position;
		agilityMath(); //I forgot to call this for a very long time after some previous commit
	}

	//A special function for agility bonuses.
	void agilityMath()
	{
		//Enemies get the same 1/100 reduction as PCs to maximum ATB.
		//But keep in mind they generally start with higher timers to begin with.
		float floatAgility = (float) charObject.agility;
		float deciFA = floatAgility / 10;
		//float centiFA = deciFA / 100;
		maxATB -= deciFA;
		currentATB = Random.Range(0f, maxATB / 4);

		//Unlike PCs, they don't get the boost to initiative.
		//For now, we're going without ambushes.
		//Player should generally get first strike, as long as they think fast.
	}

	// Update is called once per frame
	void Update ()
	{
		switch (currentState)
		{
			case (turnState.CHARGING):
				advanceATB();
				break;

			case (turnState.SELECTACTION):
				selectAction();
				break;

			case (turnState.WAITING):
				//BSM alerts ESM when action is ready
				break;

			case (turnState.ACTION):
				//The coroutine stops at a "yield",
				//but continues running again on the next frame.
				//This allows the method to work over multiple frames,
				//rather than just running all in a single frame.
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

	//note the timeActive flag
	//allows us to have wait ATB
	void advanceATB()
	{
		if (bsm.timeActive() )
		{
			currentATB = currentATB + Time.deltaTime;
			if (currentATB >= maxATB)
			{
				currentState = turnState.SELECTACTION;
			}
		}
	}

	//Same as with PCs, positive to heal and negative to hurt
	//Unlikely that we'll allow for healing (of enemies) in our final game
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

	//Unlike PCs, enemies select actions completely randomly from their pool
	//protected and virtual for now, so we could have more complicated action selection in derived enemies
	//not sure we'd ever need it for this simple project
	protected virtual void selectAction()
	{
		TurnHandler myAttack = new TurnHandler();
		//myAttack.actorName = charObject.name;
		myAttack.actorType = "Enemy";
		myAttack.actorGameObject = this.gameObject;
		myAttack.targetType = "PC";
		myAttack.targetGameObject = bsm.pcList[Random.Range(0, bsm.pcList.Count)];
		myAttack.action = charObject.skillList[Random.Range(0, charObject.skillList.Count)];

		bsm.collectAction(myAttack);
		currentState = turnState.WAITING;
	}

	public void deadCleanup()
	{
		//change tag
		this.gameObject.tag = "DeadEnemy";
		//remove as potential target for player
		bsm.enemyList.Remove(this.gameObject);

		//remove tasks from performList
		//iterate backwards to avoid index errors
		//don't allow changing of performList[0], because that is the current action in progress
		//not sure if this could lead to errors
		for (int i = bsm.performList.Count - 1; i >= 1; i--)
		{
			if (bsm.performList[i].actorGameObject == this.gameObject)
			{
				bsm.performList.RemoveAt(i);
			}
			else if (bsm.performList[i].targetGameObject = this.gameObject)
			{
				if (bsm.enemyList.Count > 0)
				{
					bsm.performList[i].targetGameObject = bsm.enemyList[Random.Range(0, bsm.enemyList.Count)];
				}
			}
		}

		//this will look weird in practice, but removes the target button successfully
		//this is even if the player is currently in menu,
		//so the button would just disappear along with the enemy dying
		if (bsm.targetSpacer.childCount > 1)
		{
			for (int i = bsm.targetSpacer.childCount - 2; i >= 0; i--)
			{
				GameObject currentButton = bsm.targetSpacer.GetChild(i).gameObject;
				GameObject currentEnemy = currentButton.transform.GetComponent<TargetButton>().target;
				//if enemy is currently in the active target menu,
				//first force the selector to off (since it can no longer be selected)
				//then destroy the button
				//(we could destroy the enemy itself too, if we wanted)
				if (currentEnemy == this.gameObject)
				{
					//currently, enemy GameObjects only have one child
					//using FindChild is unnecessary compared to GetChild(0)
					//but this might help if we change things up later
					currentEnemy.transform.Find("Selector").gameObject.SetActive(false);
					Destroy(currentButton);
					//Destroy(currentEnemy);
					break;
				}
			}
		}

		//death animation/position
		//now handled by BSM

		//Unlikely to let enemies revive, but for later use
		//Revived enemies need to wait full ATB time
		currentATB = 0f;

		alive = false;

		bsm.messageText.text += "\n" + charObject.charName + " has fallen.";
	}

	//moveToTarget is only used in the IEnumerator for handling actions
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
	//enemies get slightly more up into personal space than PCs
	//currently unused
	private bool moveToTargetShort(Vector2 targetPosition)
	{
		transform.position = Vector2.MoveTowards(transform.position, targetPosition, animSpeed * Time.deltaTime);

		//based entirely on x axis, so movement stops after an imaginary vertical line
		if (Mathf.Abs(transform.position.x - targetPosition.x) <= 4f)
		{
			return true;
		}
		//If not done moving, return false.
		return false;
	}

	//moved the action IEnumerator to the bottom of the class for easier access, kind of
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
			//It prevents the function from continuing where it left off previously.
			yield break;
		}

		actionStarted = true;
		//Trying something new, time no longer runs during animations
		bsm.animationInProgress = true;

		//move to target (maybe not necessary?)
		Vector2 targetPosition = new Vector2(actionTarget.transform.position.x - 4f, actionTarget.transform.position.y);

		//return null (wait) while moving (conditional is while not done moving)
		while (!moveToTarget(targetPosition))
		{
			yield return null;
		}

		//Pause for half a half second to think about life
		yield return new WaitForSeconds(0.125f);

		//attack
		//attackAnimation(targetPosition);
		bsm.applyDamage();

		//the rest of that quarter second
		yield return new WaitForSeconds(0.125f);

		//animations to return to start
		while (!moveToTarget(startPosition))
		{
			yield return null;
		}

		//Stop everything if battle is over
		if (bsm.battleState == StateMachineBattle.battleAction.WIN
		 && bsm.battleState == StateMachineBattle.battleAction.LOSE)
		{
			currentState = turnState.WAITING;
		}
		//If not over, remove action and go back to waiting
		else
		{
			//reset BSM to CHECK
			bsm.battleState = StateMachineBattle.battleAction.CHECK;

			actionStarted = false;

			//reset enemy to charging ATB bar
			currentATB = 0f;
			currentState = turnState.CHARGING;
		}
	}
}
