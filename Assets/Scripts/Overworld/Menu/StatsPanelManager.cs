using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsPanelManager : MonoBehaviour {

	public GameObject statDisplayTemplate;
	public Transform innerPanel;

	// Use this for initialization
	void Start() {
	}
	
	// Update is called once per frame
	// removed because we only need to update stats panel at specific times
	
	public void createPartyStats()
	{
		Debug.Log("creating stats");

		innerPanel = this.transform.GetChild(0);

		Transform partyHolder = GameObject.Find("PartyStats").transform;
		int partyCount = partyHolder.childCount;


		for (int i = 0; i < partyCount; i++)
		{
			GameObject newStatsPanel = Instantiate(statDisplayTemplate) as GameObject;
			StatsPanelData currentStatsData = newStatsPanel.GetComponent<StatsPanelData>();

			currentStatsData.gmStats = partyHolder.GetChild(i).GetComponent<BasePC>();
			currentStatsData.updateStats();
			newStatsPanel.transform.SetParent(innerPanel);
		}
	} 

	public void destroyPartyStats()
	{
		innerPanel = this.transform.GetChild(0);

		int statsPanelCount = innerPanel.childCount;

		if (statsPanelCount > 0)
		{
			for (int i = 0; i < statsPanelCount; i++)
			{
				Transform currentChildTransform = innerPanel.GetChild(i);
				Destroy(currentChildTransform.gameObject);
			}
		}
	}

	public void fullUpdate()
	{
		innerPanel = this.transform.GetChild(0);

		int partyCount = innerPanel.childCount;

		for (int i = 0; i < partyCount; i++)
		{
			StatsPanelData currentStatsPanel = innerPanel.GetChild(i).GetComponent<StatsPanelData>();
			currentStatsPanel.updateStats();
		}
	}
}
