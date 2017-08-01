using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//for saving/loading
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;

	public MenuManager mainMenu;
	public StatsPanelManager statsPanel;
	public GameObject charStatsTemplate;
	public Transform partyStats;

	public InventoryManager inventory;

	//randomising enemies
	//public List<RegionData> regionList = new List<RegionData>();
	public RegionData currentRegion;
	public List<GameObject> enemiesInBattle = new List<GameObject>();
	public int enemyAmount;
	//string for potential start message
	public string battleStartMessage = "";
	public string specialBattleScene = "";

	//the guy walking around
	public GameObject mainCharacter;
	//for battle purposes, a list of PC game objects (unused for now)
	public List<GameObject> pcList;

	//position for transitions
	public Vector2 nextCharPosition = Vector2.zero;
	public Vector2 lastCharPosition = Vector2.zero;
	//lastCharPosition was once for leaving battle and saving, mostly for cooldown now
	public string nextSpawnPoint;
	//left in for now, but it should be easy to get rid of nextSpawnPoint completely
	//just use nextCharPosition for everything

	public float battleCooldown = 0f;

	//scene names for transitions
	//public string sceneToLoad;
	public string previousScene; //for leaving battle

	//bools to handle battling transition
	public bool isWalking = false;
	public bool inDangerZone = false;
	public bool scriptedScene = false;

	//this is true when a textbox is active
	public bool inDialog = false;

	//this bool doesn't exist, but you probably think it should
	//access it via GameManager.instance.mainMenu.isActive
	//public bool menuIsActive = false;

	//public bool enemyEncountered = false;
	//the enemyEncountered bool was suggested in the tutorial I followed,
	//but I feel it's no longer necessary with the adjustments I've made
	//I've commented it out for now

	//another state machine
	//SAFEZONE used to be one of these states, but it's been removed
	//BATTLE means "starting battle"
	//"in-battle" state is IDLE, because GameManager just idles while battle happens
	public enum gameStatus {START, OVERWORLD, CUTSCENE, BATTLE, IDLE};
	public gameStatus gameState;

	// Awake instead of Start for earlier execution
	//there's a good reason for the MainCharacter instatiation happening in Awake rather than start
	void Awake ()
	{
		if (instance == null)
		{
			instance = this;
		}
		else if (instance != this)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);

		if (SceneManager.GetActiveScene().name == "StartMenu")
		{
			gameState = gameStatus.START;
		}

		//I believe what's going on here is somewhat of a hack
		//the Awake function runs every time a scene with a GameManager is entered
		//the code above tells the new GameManager to self-destruct, but this function (the Awake function) runs to completion
		//so the code below is called every time, even though we should be doing these steps in an OnSceneLoad of some sort
		//it works, but I think it would be best to fix it later if time permits
		//(I realise I'm calling myself out on poor programming practices here)
		else
		{
			gameState = gameStatus.OVERWORLD;
			if (!GameObject.Find("MainCharacter"))
			{
				GameObject mainChar = Instantiate(mainCharacter, nextCharPosition, Quaternion.identity) as GameObject;
				GameManager.instance.mainCharacter = mainChar;
				mainCharacter.name = "MainCharacter";
			}
			else
			{
				//there is some circular logic here kind of
				//pcMovement gets position from GameManager
				//GameManager gets position from current character's position, if character is already placed
				GameManager.instance.mainCharacter = GameObject.Find("MainCharacter");
				//GameManager.instance.nextCharPosition = GameManager.instance.mainCharacter.transform.position;
				//getting rid of circular logic now
			}
		}
	}

	void Start()
	{
		//UICanvas does not need to be a child of GameManager, but it makes the DontDestroyOnLoad work better
		//we'd have to refind the UICanvas on each scene change otherwise
		mainMenu = GameObject.Find("UICanvas").transform.Find("MenuPanel").GetComponent<MenuManager>();
		statsPanel = GameObject.Find("UICanvas").transform.Find("StatusPanel").GetComponent<StatsPanelManager>();
		partyStats = this.transform.Find("PartyStats");
		inventory = this.transform.Find("InventoryManager").GetComponent<InventoryManager>();
		mainMenu.menuOff();
	}

	// Update is called once per frame
	void Update()
	{
		switch (gameState)
		{
			case (gameStatus.START):
					//do nothing
					break;

			case (gameStatus.OVERWORLD):
				if (isWalking && inDangerZone && battleCooldown <= 0)
				{
					randomEncounter();
				}

				if (battleCooldown > 0 && (Vector2) mainCharacter.transform.position != lastCharPosition)
				{
					battleCooldown -= Time.deltaTime;
				}

				lastCharPosition = mainCharacter.transform.position;

				//note we check for toggle after the above checks for battles and cutscenes
				//this means no menu opening when the game is taking control for story/battle reasons
				if (Input.GetKeyDown(KeyCode.Escape) && !inDialog)
				{
					mainMenu.toggle();
				}

				break;

			case (gameStatus.CUTSCENE):
				//probably another "do nothing" here while waiting for cutscene
				//if we don't need anything, this can be combined with IDLE state
				break;
			
			//SAFEZONE seems unnecessary for now, commenting this out until later
			/*
			case (gameStatus.SAFEZONE):
				break;
			*/

			case (gameStatus.BATTLE):
				//load the battle
				startBattle();
				//then idle for a bit until battle is over
				gameState = gameStatus.IDLE;
				break;

			case (gameStatus.IDLE):
				break;
		}
	}

	public void loadScene(string sceneToLoad, bool nuke = false)
	{
		if (nuke)
		{
			Debug.Log("nuking GM");
			Destroy(gameObject);
		}
		SceneManager.LoadScene(sceneToLoad);
		Debug.Log("loaded " + sceneToLoad);
	}

	public void loadSceneAfterBattle()
	{
		loadScene(previousScene);
	} 

	//this function is unique because the game manager destroys itself after loading
	public void loadStartMenu()
	{
		loadScene("StartMenu");
		Destroy(this.gameObject);
	}

	void randomEncounter()
	{
		//if walking in danger zone and success in random roll
		//(success means success at bad luck)
		if (Random.Range(0, 1000) < 10)
		{
			//enemyEncountered = true;
			//this next part no longer makes sense, but it just demonstrates that we can start battles with messages
			//not sure we'd use this in the actual game, but I thought it seemed like a nice feature
			/*
			battleStartMessage = "Sonic is too tired to participate in battle.";
			*/
			gameState = gameStatus.BATTLE;
		}
	}

	void startBattle()
	{
		//make sure menu is off
		mainMenu.menuOff();
		//reset battle cooldown
		battleCooldown = 5f;

		//store data about where to return
		nextCharPosition = mainCharacter.transform.position;
		previousScene = SceneManager.GetActiveScene().name;

		//choose an enemy amount
		enemyAmount = Random.Range(currentRegion.minEnemyCount, currentRegion.maxEnemyCount + 1);
		//choose enemies
		for (int i = 0; i < enemyAmount; i++)
		{
			enemiesInBattle.Add(currentRegion.possibleEnemies[Random.Range(0, currentRegion.possibleEnemies.Count)]);
		}

		//load the battle
		loadScene(currentRegion.battleScene);


		//reset character bools
		isWalking = false;
		inDangerZone = false;
		//enemyEncountered = false;
	}


	public void specialBattle()
	{
		//skip BATTLE because BATTLE actually means "random battle"
		gameState = gameStatus.IDLE;
		//make sure menu is off
		mainMenu.menuOff();
		//reset battle cooldown
		battleCooldown = 5f;
		//reset character bools
		isWalking = false;
		inDangerZone = false;

		loadScene(specialBattleScene);
		//specialBattleScene = "";
	}

	//this doesn't need to take a parameter, but might as well
	//could be alternative "new games" like in a scenario selector
	//mostly writing this to further fix errors in the handling of items
	public void startNewGame(string sceneName)
	{
		//here's the fix
		UnLockable.doorTracker = new Dictionary<int, bool>();
		Collectible.keyCollection = new Dictionary<int, bool>();

		//this shouldn't be necessary for new games, but might as well
		MusicNoStop.AudioBegin = false;

		gameState = gameStatus.OVERWORLD;
		loadScene(sceneName);
	}

	public void exitToStartMenu()
	{
		mainMenu.menuOff();
		Destroy(gameObject);
		//next line isn't the best way, but whatever
		//can make more generic later
		Destroy(GameObject.Find("Music"));

		//reinitialising music script
		MusicNoStop.AudioBegin = false;

		loadScene("StartMenu");
	}

	public void saveGame()
	{
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/saveData.dat");

		saveData data = writeSaveToStruct();

		bf.Serialize(file, data);
		file.Close();
	}


	public void loadGame()
	{
		if (File.Exists(Application.persistentDataPath + "/saveData.dat"))
		{
			Debug.Log("attemping load");

			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/saveData.dat", FileMode.Open);

			saveData data = (saveData)bf.Deserialize(file);
			file.Close();

			loadSaveFromStruct(data);

			if (data.lastScene == "TownTest2D" || data.lastScene == "FieldTest2D")
			{
				hideInventory();
			}

			loadScene(data.lastScene);
			gameState = gameStatus.OVERWORLD;
		}

		else
		{
			Debug.Log("no file to load man");
			//loadScene("StartMenu");
		}
	}

	//going to move out some of the above code here for simplicity
	public saveData writeSaveToStruct()
	{
		saveData data = new saveData();

		Transform partyHolder = GameObject.Find("PartyStats").transform;
		//lastCharPosition = GameObject.Find("MainCharacter").transform.position;

		data.party = new List<saveStats>();
		for (int i = 0; i < partyHolder.childCount; i++)
		{
			BasePC currentPC = partyHolder.GetChild(i).GetComponent<BasePC>();
			saveStats currentStats = new saveStats();

			currentStats.skills = new List<string>();

			currentStats.charName = currentPC.charName;

			currentStats.level = currentPC.level;
			currentStats.experience = currentPC.experience;

			currentStats.maxHP = currentPC.maxHP;
			currentStats.currentHP = currentPC.currentHP;
			currentStats.maxMP = currentPC.maxMP;
			currentStats.currentMP = currentPC.currentMP;

			currentStats.strength = currentPC.strength;
			currentStats.magic = currentPC.magic;
			currentStats.defense = currentPC.defense;
			currentStats.magdef = currentPC.magdef;
			currentStats.agility = currentPC.agility;
			currentStats.luck = currentPC.luck;

			for (int j = 0; j < currentPC.skillList.Count; j++)
			{
				//note we're saving skill object name, not in-game skill name
				//some skills have different prefab names from their in-game names
				Debug.Log(currentPC.skillList[j].name);
				currentStats.skills.Add(currentPC.skillList[j].name);
			}

			currentStats.baseAttack = currentPC.baseAttack.actionName;

			data.party.Add(currentStats);
		}
		data.posx = lastCharPosition.x;
		data.posy = lastCharPosition.y;

		data.lastScene = SceneManager.GetActiveScene().name;

		data.inventory = inventory.inventoryDict;

		data.doorDict = UnLockable.doorTracker;
		data.keyDict = Collectible.keyCollection;

		return data;
	}

	public void loadSaveFromStruct(saveData data)
	{
		//first get the position out, because that's the easy part
		nextCharPosition = new Vector2(data.posx, data.posy);

        //if party members already exist, nuke them

        int currentPartyCount = partyStats.childCount;

		for (int i = currentPartyCount - 1; i >= 0; i--)
		{
			Destroy(partyStats.GetChild(i).gameObject);
		}


		for (int i = 0; i < data.party.Count; i++)
		{
			GameObject newCharStats = Instantiate(charStatsTemplate) as GameObject;
			saveStats currentSaveStats = data.party[i];
			BasePC currentGameStats = newCharStats.transform.GetComponent<BasePC>();

			currentGameStats.charName = currentSaveStats.charName;
			newCharStats.name = currentGameStats.charName;

			currentGameStats.level = currentSaveStats.level;
			currentGameStats.experience = currentSaveStats.experience;

			currentGameStats.maxHP = currentSaveStats.maxHP;
			currentGameStats.currentHP = currentSaveStats.currentHP;
			currentGameStats.maxMP = currentSaveStats.maxMP;
			currentGameStats.currentMP = currentSaveStats.currentMP;

			currentGameStats.strength = currentSaveStats.strength;
			currentGameStats.magic = currentSaveStats.magic;
			currentGameStats.defense = currentSaveStats.defense;
			currentGameStats.magdef = currentSaveStats.magdef;
			currentGameStats.agility = currentSaveStats.agility;
			currentGameStats.luck = currentSaveStats.luck;

			//not sure this will work, but we'll find out
			GameObject baObject = (GameObject)Resources.Load("Actions/" + currentSaveStats.baseAttack, typeof(GameObject));
			currentGameStats.baseAttack = baObject.transform.GetComponent<BaseAction>();

			for (int j = 0; j < currentSaveStats.skills.Count; j++)
			{
				GameObject skillObject = (GameObject)Resources.Load("Actions/" + currentSaveStats.skills[j], typeof(GameObject));
				currentGameStats.skillList.Add(skillObject.GetComponent<BaseAction>());
			}

			newCharStats.transform.SetParent(partyStats);
		}

		//moved all inventory stuff to the bottom here
		// Load Inventory Data from Dictionary - Init text based on capacity
		inventory.inventoryDict = data.inventory;

		inventory.YkeyCountText.text = "     Yellow Keys: " + inventory.inventoryDict["YKey"];
		inventory.GkeyCountText.text = "     Green Keys: " + inventory.inventoryDict["GKey"];
		inventory.RkeyCountText.text = "     Red Keys: " + inventory.inventoryDict["RKey"];
		inventory.passCardCountText.text = "     Pass Cards: " + inventory.inventoryDict["PCard"];

		UnLockable.doorTracker = data.doorDict;
		Collectible.keyCollection = data.keyDict;
	}

	//bonus function
	public void loadDemo()
	{
		GameManager.instance.gameState = gameStatus.OVERWORLD;
		Destroy(transform.Find("PartyStats").gameObject);
		GameObject party = (GameObject)Resources.Load("demoParty/PartyStats", typeof(GameObject));
		GameObject newParty = Instantiate(party, transform);
		newParty.name = "PartyStats";

		hideInventory();

		loadScene("TownTest2D");
	}

	public void hideInventory()
	{
		Destroy(transform.Find("UICanvas/MenuPanel/MenuPanelSpacer/InventoryButton").gameObject);
		Destroy(transform.Find("UICanvas/InventoryPanel").gameObject);
		//Destroy(GameObject.Find("InventoryManager"));
		transform.Find("UICanvas/MenuPanel/MenuPanelSpacer/SaveButton").gameObject.GetComponent<MouseoverTexter>().mouseoverText
		= "Saving here WILL overwrite your main story save.";
	}
}


//saveStats is like a serializable-friendly version of BasePC
//saving this way seems to work, though we still need to parse the names back when loading
//http://answers.unity3d.com/questions/674127/how-to-find-a-prefab-via-script.html
//that link might provide some hints to how we'd load
//loading is working now, so check the link below instead

//https://forum.unity3d.com/threads/webgl-filesystem.294358/#post-1940712
//this should be a simple fix that makes things work on webGL builds
//we can implement it later if we want, but we might need some preprocessor conditionals or something
//it's strictly for web builds, not offline ones (calls external JS functions outside of Unity)
[System.Serializable]
public struct saveStats
{
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

	public List<string> skills;
	public string baseAttack;
}

[System.Serializable]
public struct saveData
{
	public List<saveStats> party;
	public Dictionary<string, int> inventory;
	public float posx;
	public float posy;
	public string lastScene;

	public Dictionary<int, bool> doorDict;
	public Dictionary<int, bool> keyDict;
}
