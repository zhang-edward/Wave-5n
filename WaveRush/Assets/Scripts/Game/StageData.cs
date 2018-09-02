using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Data", menuName = "Stages/StageData", order = 1)]
public class StageData : ScriptableObject
{
	[Serializable]
	public class EnemySpawnProperties {
		public GameObject prefab;
		public float spawnFrequency = 1.0f;		// if this enemy is selected to spawn, this is the chance that it will spawn
		public int waveLimit = 1;				// the enemy will only be available to spawn after this wave
	}

	[Serializable]
	public class WaveProperties {
		public enum EnemySpawnMode {
			Random,
			Beginning,
			End
		}
		[Serializable]
		public struct WaveSpecificEnemyProperties {
			
			public GameObject prefab;
			public int numEnemies;
			public EnemySpawnMode spawnMode;		// 0 = Spawned at beginning, 1 = Spawned randomly throughout, 2 = Spawned at end
		}
		public int waveNumber;
		public int numEnemies = 15;
		public int maxNumActiveEnemies = 8;
		public List<WaveSpecificEnemyProperties> enemies;
	}

	public DialogueSet[] dialogueSets;
	[Header("Stage Properties")]
	public string stageName;
	public MapType mapType;
	public int difficultyLevel;
	public int maxPartySize = 1;
	public HeroTier maxTier = HeroTier.tier3;
	[Range(1, 50)] public int levelRaw = 1;
	[Header("Difficulty Curve Variables")]
	public float upperLimit = 15;					// The maximum number of enemies possible
	public float lowerLimit = 5;					// The minimum number of enemies possible
	[Header("Stage Events")]
	public int goalWave = 5;                            // What wave the player must reach for the stage to be completed
	public int bossWave = 5;
	[Header("Enemies")]
	public List<EnemySpawnProperties> enemyPrefabs;  	// set of all enemies spawnable for this stage
	public List<GameObject> bossPrefabs;   				// bosses
	[Header("Waves")]
	public List<WaveProperties> waveProperties;

	


	// retrieve the list of available enemies for spawning based on the amount of points available
	public List<EnemySpawnProperties> GetRandomEnemyPool(int waveNumber)
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

	public List<WaveProperties.WaveSpecificEnemyProperties> GetSpecificEnemyPool(int waveNumber) {
		return GetWaveProperties(waveNumber).enemies;
	}

	public WaveProperties GetWaveProperties(int waveNumber) {
		UnityEngine.Assertions.Assert.IsTrue(waveProperties[0].waveNumber == 0);
		foreach (WaveProperties properties in waveProperties) {
			if (properties.waveNumber == waveNumber)
				return properties;
		}
		return waveProperties[0];
	}
} 