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
	public GameObject moneyPickup;

	public int waveNumber { get; private set; }
	private int difficultyCurve = 0;	// number to determine the number of enemies to spawn
	public ShopNPC shopNPC;

	public delegate void EnemyWaveSpawned (int waveNumber);
	public event EnemyWaveSpawned OnEnemyWaveSpawned;
	public delegate void EnemyWaveCompleted ();
	public event EnemyWaveCompleted OnEnemyWaveCompleted;
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
				if (waveNumber >= 1)
				{
					OnEnemyWaveCompleted ();
				}
				waveNumber++;
				difficultyCurve++;
				TrySpawnBoss ();
				// if it is the wave after a boss wave (just defeated boss), spawn heart pickup
				if (waveNumber % bossWave == 1 && waveNumber != 1)
				{
					Instantiate (heartPickup, map.CenterPosition, Quaternion.identity);

					shopNPC.Appear ();
					while (shopNPC.gameObject.activeInHierarchy)
						yield return null;
				}

				StartNextWave ();
			}
			yield return null;
		}
	}

	private void TrySpawnBoss()
	{
		// every 'bossWave' waves, spawn a boss
		if (waveNumber % bossWave == 0)
		{
			difficultyCurve -= onBossDifficultyScaleBack;
			if (difficultyCurve <= 0)
				difficultyCurve = 1;
			Invoke ("StartBossIncoming", 5.0f);
		}
	}

	private void StartNextWave()
	{
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
		{
			Vector3 randOpenCell = (Vector3)map.OpenCells [Random.Range (0, map.OpenCells.Count)];
			SpawnEnemy (prefabPool [Random.Range (0, prefabPool.Count)], randOpenCell);
		}
		Debug.Log ("Number of enemies in this wave: " + numToSpawn);
	}

	private void StartBossIncoming()
	{
		OnBossIncoming ();
		Invoke ("SpawnBoss", bossSpawnDelay);
	}

	public void SpawnEnemy(GameObject prefab, Vector3 pos)
	{
		GameObject o = Instantiate (prefab);
		o.transform.SetParent (transform);
		if (Random.value < 0.5f)
			o.transform.position = new Vector3 (Random.Range (0, 10), Map.size + 4);
		else
			o.transform.position = new Vector3 (Random.Range (0, 10), -4);

		Enemy e = o.GetComponentInChildren<Enemy> ();
		EnemyHealthBar healthBar = enemyHealthBarPool.GetPooledObject ().GetComponent<EnemyHealthBar>();
		healthBar.Init (e);
		healthBar.player = player;

		e.Init (pos, map);
		e.moneyPickupPrefab = moneyPickup;
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
		e.moneyPickupPrefab = moneyPickup;
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
