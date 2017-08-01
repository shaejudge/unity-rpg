using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanelManager : MonoBehaviour {

    public GameObject inventoryPanelTemplate;
    public GameObject inventoryButtonTemplate;
    public GameObject targetButtonTemplate;
    public GameObject cancelButtonTemplate;

    public GameObject inventoryDescPanel;
    public Text inventoryItemText;

    public Transform targetInnerPanel;

    public Transform innerPanel;

    public bool isActive = false;

    public SkillHandler playerChoice;



    // Use this for initialization
    void Start () {
        deactivateInventoryDescPanel();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void toggle()
    {
        if (isActive)
        {
            deactivateInventoryPanel();
        }
        else
        {
            activateInventoryPanel();
        }
    }

    public void activateInventoryPanel()
    {
        this.gameObject.SetActive(true);
        playerChoice = new SkillHandler(); //this is done here so we have a fresh skillHandler when opening
        isActive = true;
    }

    public void deactivateInventoryPanel()
    {
        this.gameObject.SetActive(false);
        isActive = false;
    }

    public void deactivateInventoryDescPanel()
    {
        inventoryDescPanel.SetActive(false);
    }
    
    public void activateInventoryDescPanel(string message)
    {
        inventoryItemText.text = message;
        inventoryDescPanel.SetActive(true);
    }

}
