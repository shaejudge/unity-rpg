using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RegionData : MonoBehaviour {

	public string regionName;
	public int minEnemyCount = 1;
	public int maxEnemyCount = 4;
	public string battleScene;
	public List<GameObject> possibleEnemies = new List<GameObject>();
}
