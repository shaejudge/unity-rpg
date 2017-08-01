using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {

    public InventoryManager theInventoryManager;

    public string itemToIncrement;

    //private static int keyTracker = 0;
    public int thisKeyID = -1;

    public static Dictionary<int, bool> keyCollection;

    private void Awake()
    {
        if (keyCollection == null)
        {
            keyCollection = new Dictionary<int, bool>();
        }
    }

    // Use this for initialization
    void Start () {
		theInventoryManager = GameManager.instance.transform.Find("InventoryManager").GetComponent<InventoryManager>();
        
        if (keyCollection.ContainsKey(thisKeyID))
        {
            if (keyCollection[thisKeyID])
            {
                Destroy(gameObject);
            } else
            {
                keyCollection.Add(thisKeyID, false);
            }
        }

    }

    // Update is called once per frame
    void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D character)
    {
        if (character.gameObject.name == "MainCharacter")
        {
            if (string.Compare(itemToIncrement, "YKey") == 0)
            {
                theInventoryManager.IncrementItem(itemToIncrement);
            } else if (string.Compare(itemToIncrement, "GKey") == 0)
            {
                theInventoryManager.IncrementItem(itemToIncrement);
            } else if (string.Compare(itemToIncrement, "RKey") == 0)
            {
                theInventoryManager.IncrementItem(itemToIncrement);
                
            } else if (string.Compare(itemToIncrement, "PCard") == 0)
            {
                theInventoryManager.IncrementItem(itemToIncrement);
            }

            collectedKeys();
            gameObject.SetActive(false);
        }
    }

    void collectedKeys()
    {
        keyCollection[thisKeyID] = true;
    }
}
