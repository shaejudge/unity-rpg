using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script appears to be unused

public class UseInventory : MonoBehaviour {

    //private InventoryManager theInventoryManager;
    private UnLockable unLockableItems;

    public string itemToDecrement;
    public string itemToUnlock;

	// Use this for initialization
	void Start () {
        //theInventoryManager = FindObjectOfType<InventoryManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /*
    void OnTriggerEnter2D(Collider2D character)
    {
        if (character.gameObject.name == "MainCharacter")
        {
            Debug.Log("In Collision with: " + itemToUnlock);
            unLockableItems.UnLock(itemToUnlock);

            if (string.Compare(itemToDecrement, "Key") == 0)
            {
                //unLockableItems.UnLock(itemToUnlock);
                theInventoryManager.DecrementItem(itemToDecrement);
            }
            if (string.Compare(itemToDecrement, "Diamond") == 0)
            {
                theInventoryManager.DecrementItem(itemToDecrement);
            }
            if (string.Compare(itemToDecrement, "Mushroom") == 0)
            {
                theInventoryManager.DecrementItem(itemToDecrement);
            }
        }
    }*/
}
