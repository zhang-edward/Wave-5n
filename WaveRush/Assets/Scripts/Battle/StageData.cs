using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "Stages/StageData", order = 1)]
public class StageData : ScriptableObject
{
	[System.Serializable]
	public class EnemySpawnProperties
	{
		public GameObject prefab;
		public float spawnFrequency = 1.0f;		// if this enemy is selected to spawn, this is the chance that it will spawn
		public int waveLimit = 1;				// the enemy will only be available to spawn after this wave
	}
	public string stageName;
	public MapType mapType;
	public int level;
	public List<EnemySpawnProperties> enemyPrefabs;  	// set of all enemies spawnable for this stage
	public List<GameObject> bossPrefabs;   				// bosses


	// retrieve the list of available enemies for spawning based on the amount of points available
	public List<GameObject> GetSpawnList(int waveNumber)
	{
		List<GameObject> answer = new List<GameObject>();
		foreach (EnemySpawnProperties enemyProp in enemyPrefabs)
		{
			Enemy enemy = enemyProp.prefab.GetComponentInChildren<Enemy>();
			if (enemyProp.waveLimit <= waveNumber)
			{
				answer.Add(enemyProp.prefab);
			}
		}
		return answer;
	}
} 