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
	public DialogueSet[] dialogueSets;
	[Header("Stage Properties")]
	public string stageName;
	public MapType mapType;
	public int level;
	[Header("Difficulty Curve Variables")]
	public float upperAsymptote = 15;					// The maximum number of enemies possible
	public float lowerAsymptote = 5;					// The minimum number of enemies possible
	public float maxDifficultyVelocity = 5;				// The wave for which the difficulty is increasing the most
	[Header("Stage Events")]
	public int goalWave = 5;                            // what wave the player must reach for the stage to be completed
	//public int shopWave = 3;
	public int bossWave = 5;
	[Header("Enemies")]
	public List<EnemySpawnProperties> enemyPrefabs;  	// set of all enemies spawnable for this stage
	public List<GameObject> bossPrefabs;   				// bosses


	// retrieve the list of available enemies for spawning based on the amount of points available
	public List<EnemySpawnProperties> GetSpawnList(int waveNumber)
	{
		List<EnemySpawnProperties> answer = new List<EnemySpawnProperties>();
		foreach (EnemySpawnProperties enemyProp in enemyPrefabs)
		{
			Enemy enemy = enemyProp.prefab.GetComponentInChildren<Enemy>();
			if (enemyProp.waveLimit <= waveNumber)
			{
				answer.Add(enemyProp);
			}
		}
		return answer;
	}
} 