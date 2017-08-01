using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnLockable : MonoBehaviour {

    private InventoryManager theInventoryManager;
    public string lockedItem;
    public string requires;
    public string message;
    public Text doorText;

    public int thisDoorID;

    // Create enum for lockables 
    enum LockedItems { YDoor, GDoor, RDoor, PDoor }

    public static Dictionary<int, bool> doorTracker;

    void Awake()
    {
        if (doorTracker == null)
        {
            doorTracker = new Dictionary<int, bool>();
        }
    }

    // Use this for initialization
    void Start () {
        theInventoryManager = GameManager.instance.transform.Find("InventoryManager").GetComponent<InventoryManager>();

        doorText.enabled = false;

        if (doorTracker.ContainsKey(thisDoorID))
        {
            if (doorTracker[thisDoorID])
            {
                Destroy(gameObject);
            } else
            {
                doorTracker.Add(thisDoorID, false);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void UnLock(string lockedItem)
    {
        if (System.Enum.IsDefined(typeof(LockedItems), lockedItem))
        {
            gameObject.SetActive(false);
            OpenDoor();
        }
    }

    IEnumerator ShowMessage(string message, float delay)
    {
        doorText.text = "Door requires a " + message;
        doorText.enabled = true;
        yield return new WaitForSeconds(delay);
        doorText.enabled = false;
    }

    void OnCollisionEnter2D(Collision2D character)
    {
        if (character.gameObject.name == "MainCharacter")
        {
            if (System.Enum.IsDefined(typeof(LockedItems), lockedItem))
            {
                if (theInventoryManager.inventoryDict[requires] > 0)
                {
                    gameObject.GetComponent<BoxCollider2D>().isTrigger = true;
                    UnLock(lockedItem);
                    theInventoryManager.DecrementItem(requires);
                }
                else
                {
                    StartCoroutine(ShowMessage(message, 2));
                }
            }
        }
    }

    void OpenDoor()
    {
        doorTracker[thisDoorID] = true;
    }
}
