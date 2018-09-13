using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class IncomingEnemiesView : MonoBehaviour {

	public ObjectPooler enemyIconPool;
	public Dictionary<GameObject, int> enemiesDict = new Dictionary<GameObject, int>();
	public EnemyManager enemyManager;
	public GameObject scrollableIndicatorGraphic;
	public int maxColumnCount = 6;

	void OnEnable() {
		Init(enemyManager.enemyQueue);
	}

	public void Init(Queue<GameObject> enemyQueue) {
		enemyIconPool.Init();
		// Reset all enemy icons and dictionary
		enemiesDict.Clear();
		foreach (GameObject o in enemyIconPool.GetAllActiveObjects()) {
			o.SetActive(false);
		}
		// Retrieve enemies from enemyQueue
		foreach (GameObject o in enemyQueue) {
			if (enemiesDict.ContainsKey(o))
				enemiesDict[o] ++;
			else
				enemiesDict.Add(o, 1);		
		}
		// Get enemy icons from dictionary
		foreach (KeyValuePair<GameObject, int> kvp in enemiesDict) {
			GameObject o = kvp.Key;
			int count = kvp.Value;
			GameObject iconObject = enemyIconPool.GetPooledObject();
			iconObject.gameObject.SetActive(true);
			EnemyIcon icon = iconObject.GetComponent<EnemyIcon>();
			icon.Init(o.GetComponentInChildren<Enemy>(), count);
		}

		// If the view is scrollable, set the graphic that indicates it to the player
		if (enemiesDict.Count > maxColumnCount) {
			scrollableIndicatorGraphic.SetActive(true);
		}
		else {
			scrollableIndicatorGraphic.SetActive(false);
		}
	}
}