using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {

	//a simple bool to keep track of activeness for now
	public bool isActive = false;

	//trying something new here with the skill descriptions
	public GameObject messagePanel;

	//in essence, initialising this part in the editor to avoid possible timing conflicts with start/awake
	public GameObject statusPanel;
	//this next part is initialised in code
	public StatsPanelManager statusPanelManager;

	public GameObject skillsButton;
	private Text skillsButtonText;

	public GameObject skillPanel;
	public SkillPanelManager skillPanelManager;

    public GameObject inventoryButton;
    private Text inventoryButtonText;

    public GameObject inventoryPanel;
    public InventoryPanelManager inventoryPanelManager;

	// Use this for initialization
	void Start () {
		skillPanelManager = skillPanel.GetComponent<SkillPanelManager>();
		if (inventoryPanel != null)
		{
			inventoryPanelManager = inventoryPanel.GetComponent<InventoryPanelManager>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void toggle()
	{
		//this may seem unnecessary, but having distinct on/off methods helps for forcing things on or off when needed
		//for example, the menu should always be off in battle
		Debug.Log("toggled");
		if (isActive)
		{
			menuOff();
		}
		else
		{
			menuOn();
		}
	}

	public void menuOn()
	{
		isActive = true;
		statusPanelManager = statusPanel.GetComponent<StatsPanelManager>();
		statusPanelManager.createPartyStats();
		this.gameObject.SetActive(true);
		statusPanel.SetActive(true);
	}

	public void menuOff()
	{
		isActive = false;
		this.gameObject.SetActive(false);
		statusPanel.SetActive(false);
		statusPanelManager = statusPanel.GetComponent<StatsPanelManager>();
		//used to have a "if innerpanel not null" check here, but shouldn't be needed
		//leaving this comment in case it ends up being needed because of initialisation weirdness
		statusPanelManager.destroyPartyStats();
		skillPanelManager = skillPanel.GetComponent<SkillPanelManager>();
		skillPanelManager.deactivateSkillPanel();
		skillPanelManager.deactivateSkillDescPanel();
		skillsButton.transform.GetChild(0).GetComponent<Text>().text = "Show Skills";

		if (inventoryPanel != null)
		{
			inventoryPanelManager = inventoryPanel.GetComponent<InventoryPanelManager>();
			inventoryPanelManager.deactivateInventoryPanel();
			inventoryPanelManager.deactivateInventoryDescPanel();
			inventoryButton.transform.GetChild(0).GetComponent<Text>().text = "Show Inventory";
		}
	}

	public void exitToStart()
	{
		Debug.Log("exited");
		GameManager.instance.exitToStartMenu();
	}

	//a wrapper that only calls the main saveGame function
	public void saveGame()
	{
		GameManager.instance.saveGame();
	}

	public void skillButton()
	{
		skillPanelManager = skillPanel.GetComponent<SkillPanelManager>();
		skillPanelManager.toggle();
		skillsButtonText = skillsButton.transform.GetChild(0).GetComponent<Text>();
		if (skillPanelManager.isActive)
		{

			skillsButtonText.text = "Hide Skills";
		}
		else
		{

			skillsButtonText.text = "Show Skills";
		}
	}

    public void showInventoryButton()
    {
        inventoryPanelManager = inventoryPanel.GetComponent<InventoryPanelManager>();
        inventoryPanelManager.toggle();
        inventoryButtonText = inventoryButton.transform.GetChild(0).GetComponent<Text>();
        if (inventoryPanelManager.isActive)
        {

            inventoryButtonText.text = "Hide Inventory";
        }
        else
        {

            inventoryButtonText.text = "Show Inventory";
        }
    }
}
