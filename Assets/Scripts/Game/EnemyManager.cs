using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	private const float DIFFICULTY_CURVE = 3.5f;

	public Player player;
	public int enemiesKilled { get; private set; }
	public Map map;

	public ObjectPooler enemyHealthBarPool;

	private List<Enemy> enemies = new List<Enemy>();
	public List<Enemy> Enemies { get; private set; }
	public EnemyManagerInfo info;

	public EnemyHealthBar bossHealthBar;
	private BossSpawn bossSpawn;
	public int bossWave = 2;
	public float bossSpawnDelay = 3f;
	public int onBossDifficultyScaleBack = 3;	// subtract this from difficultyCurve on a boss wave

	public GameObject heartPickup;

	public int waveNumber { get; private set; }
	private int difficultyCurve = 0;	// number to determine the number of enemies to spawn

	public delegate void EnemyWaveSpawned (int waveNumber);
	public event EnemyWaveSpawned OnEnemyWaveSpawned;
	public delegate void BossIncoming();
	public event BossIncoming OnBossIncoming;

	void OnEnable()
	{
		player.OnPlayerInitialized += Init;
	}

	void OnDisbled()
	{
		player.OnPlayerInitialized -= Init;
	}

	private void Init()
	{
		bossSpawn = map.bossSpawn.GetComponent<BossSpawn> ();
		StartCoroutine (StartSpawningEnemies ());
	}

	private IEnumerator StartSpawningEnemies()
	{
		while (true)
		{
			if (NumAliveEnemies() <= 0)
			{
				waveNumber++;
				difficultyCurve++;

				// every 'bossWave' waves, spawn a boss
				if (waveNumber % bossWave == 0)
				{
					difficultyCurve -= onBossDifficultyScaleBack;
					if (difficultyCurve <= 0)
						difficultyCurve = 1;
					Invoke ("StartBossIncoming", 5.0f);
				}
				// if it is the wave after a boss wave (just defeated boss), spawn heart pickup
				if (waveNumber % bossWave == 1 && waveNumber != 1)
				{
					Instantiate (heartPickup, map.CenterPosition, Quaternion.identity);
				}
				if (OnEnemyWaveSpawned != null)
				{
					OnEnemyWaveSpawned (waveNumber);
				}

				// Number of enemies spawning curve (used desmos.com for the graph)
				int numToSpawn = Mathf.RoundToInt (DIFFICULTY_CURVE * Mathf.Log (difficultyCurve) + 5);
				List<GameObject> prefabPool = new List<GameObject>();
				if (waveNumber <= 5)
				{
					prefabPool = info.enemyPrefabs1;
				}
				else if (5 < waveNumber && waveNumber <= 10)
				{
					prefabPool.AddRange (info.enemyPrefabs1);
					prefabPool.AddRange (info.enemyPrefabs2);
				}
				else
				{
					prefabPool = info.enemyPrefabs2;
				}

				for (int i = 0; i < numToSpawn; i++)
					SpawnEnemy (prefabPool);
				

				Debug.Log ("Number of enemies in this wave: " + numToSpawn);
			}
			yield return null;
		}
	}

	private void StartBossIncoming()
	{
		OnBossIncoming ();
		Invoke ("SpawnBoss", bossSpawnDelay);
	}

	public void SpawnEnemy(List<GameObject> prefabPool)
	{
		Vector3 randOpenCell = (Vector3)map.OpenCells [Random.Range (0, map.OpenCells.Count)];
		GameObject o = Instantiate (prefabPool [Random.Range (0, prefabPool.Count)]);
		o.transform.SetParent (transform);
		if (Random.value < 0.5f)
			o.transform.position = new Vector3 (Random.Range (0, 10), Map.size + 4);
		else
			o.transform.position = new Vector3 (Random.Range (0, 10), -4);

		Enemy e = o.GetComponentInChildren<Enemy> ();
		EnemyHealthBar healthBar = enemyHealthBarPool.GetPooledObject ().GetComponent<EnemyHealthBar>();
		healthBar.Init (e);
		healthBar.player = player;

		e.Init (randOpenCell, map);
		e.player = player.transform;
		enemies.Add (e);
		e.OnEnemyDied += IncrementEnemiesKilled;
	}

	public void SpawnBoss()
	{
		bossSpawn.PlayAnimation ();
		GameObject o = Instantiate (info.bossPrefabs [Random.Range (0, info.bossPrefabs.Count)]);
		o.transform.SetParent (transform);
		Enemy e = o.GetComponentInChildren<Enemy> ();
		e.Init (bossSpawn.transform.position, map);
		e.player = player.transform;
		bossHealthBar.Init (e);
		enemies.Add (e);
	}

	private int NumAliveEnemies()
	{
		int count = 0;
		for (int i = enemies.Count - 1; i >= 0; i --)
		{
			Enemy e = enemies [i];
			// simultaneously clean list
			if (e == null)
				enemies.Remove (e);
			// count alive enemies
			if (e.health > 0)
				count++;
		}
		return count;
	}

	private void IncrementEnemiesKilled()
	{
		enemiesKilled++;	
	}
}
