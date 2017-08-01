using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {

    // A few example inventory items; adjust as necessary
    public Text YkeyCountText;
    public Text GkeyCountText;
    public Text RkeyCountText;
    public Text passCardCountText;

    // Per Albert's request: inventory stored in dictionary
    // Add new categories in Awake()
    public Dictionary<string, int> inventoryDict;



	// Use this for initialization
	void Awake () {
        inventoryDict = new Dictionary<string, int>();
        inventoryDict.Add("YKey", 0);
        inventoryDict.Add("GKey", 0);
        inventoryDict.Add("RKey", 0);
        inventoryDict.Add("PCard", 0);

        //DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void IncrementItem(string itemToAdd)
    {
        if (string.Compare(itemToAdd, "YKey") == 0)
        {
            inventoryDict[itemToAdd] += 1;
            YkeyCountText.text = "     Yellow Keys: " + inventoryDict[itemToAdd];
        }
        else if (string.Compare(itemToAdd, "GKey") == 0)
        {
            inventoryDict[itemToAdd] += 1;
            GkeyCountText.text = "     Green Keys: " + inventoryDict[itemToAdd];
        }
        else if (string.Compare(itemToAdd, "RKey") == 0)
        {
            inventoryDict[itemToAdd] += 1;
            RkeyCountText.text = "     Red Keys: " + inventoryDict[itemToAdd];
        }
        else if (string.Compare(itemToAdd, "PCard") == 0)
        {
            inventoryDict[itemToAdd] += 1;
            passCardCountText.text = "     Pass Cards: " + inventoryDict[itemToAdd];
        }
    }

    public void DecrementItem(string itemToDecrement)
    {
        Debug.Log("Decrement: " + itemToDecrement);
        if (string.Compare(itemToDecrement, "YKey") == 0)
        {
            inventoryDict[itemToDecrement] -= 1;
            YkeyCountText.text = "     Yellow Keys: " + inventoryDict[itemToDecrement];
        }
        else if (string.Compare(itemToDecrement, "GKey") == 0)
        {
            inventoryDict[itemToDecrement] -= 1;
            GkeyCountText.text = "     Green Keys: " + inventoryDict[itemToDecrement];
        }
        else if (string.Compare(itemToDecrement, "RKey") == 0)
        {
            inventoryDict[itemToDecrement] -= 1;
            RkeyCountText.text = "     Red Keys: " + inventoryDict[itemToDecrement];
        }
        else if (string.Compare(itemToDecrement, "PCard") == 0)
        {
            inventoryDict[itemToDecrement] -= 1;
            passCardCountText.text = "     Pass Cards: " + inventoryDict[itemToDecrement];
        }
    }

}
