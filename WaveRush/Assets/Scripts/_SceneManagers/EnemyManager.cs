using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	public const float WAVE_SPAWN_DELAY = 10.0f;

	[Header("Set from Scene Hierarchy")]
	public Player player;
	public int enemiesKilled { get; private set; }
	public Map map;

	public ObjectPooler enemyHealthBarPool;

	private List<Enemy> enemies = new List<Enemy>();
	public List<Enemy> Enemies { get {return enemies;} }

	public int level;
	public StageData stageData { get; private set; }

	public EnemyHealthBar bossHealthBar;
	private BossSpawn bossSpawn;
	private float bossSpawnDelay = 8f;
	private bool hasBossSpawned = true;

	[Header("Pickups and Prefabs")]
	public GameObject heartPickup;
	public GameObject upgradePickup;
	public GameObject trappedHeroPrefab;
	public GameObject endPortalPrefab;
	[Header("EnemyManager Properties")]
	public bool paused;
	public int waveNumber { get; private set; }
	public bool isStageComplete { get; private set; }
	private int difficultyCurve = 0;	// number to determine the number of enemies to spawn
	[Header("Effects")]
	public SimpleAnimation bossDeathEffect;

	// Hidden properties
	[HideInInspector] public List<BossEnemy> bosses;
	[HideInInspector] public float timeLeftBeforeNextWave; 

	public delegate void EnemyWaveSpawned (int waveNumber);
	public event EnemyWaveSpawned OnEnemyWaveSpawned;
	public delegate void EnemyManagerEvent ();
	public event EnemyManagerEvent OnEnemyWaveCompleted;
	public event EnemyManagerEvent OnStageCompleted;
	public event EnemyManagerEvent OnEndPortalSpawned;
	public event EnemyManagerEvent OnQueueBossMessage;
	public event EnemyManagerEvent OnEnemyDefeated;

	public void Init(StageData data)
	{
		this.stageData = data;
		this.level = data.levelRaw;
		UnityEngine.Assertions.Assert.IsTrue(level > 0);
		bossSpawn = map.bossSpawn.GetComponent<BossSpawn> ();
		StartCoroutine (StartSpawningEnemies ());
	}

	private IEnumerator StartSpawningEnemies()
	{
		// Wait for GUIManager to display its first message
		yield return new WaitForSeconds(2f);
		for (;;)
		{
			while (paused)
				yield return null;
			// all enemies dead
			if (NumAliveEnemies() <= 0 && hasBossSpawned)
			{
				if (waveNumber >= 1)
				{
					// If the stage is completed
					if (waveNumber % stageData.goalWave == 0)
					{
						if (OnStageCompleted != null)
							OnStageCompleted();
						isStageComplete = true;
						while (paused)
							yield return null;

						isStageComplete = false;
					}
					// If the stage is not completed
					else {
						if (OnEnemyWaveCompleted != null)
							OnEnemyWaveCompleted();
						
						timeLeftBeforeNextWave = WAVE_SPAWN_DELAY;
						while (timeLeftBeforeNextWave > 0) {
							timeLeftBeforeNextWave -= Time.deltaTime;
							yield return null;
						}
					}
				}

				waveNumber++;
				difficultyCurve++;
				// Spawn a heart pickup after each boss
				if (waveNumber % stageData.bossWave == 1 && waveNumber != 1) {
					Instantiate(heartPickup, map.CenterPosition, Quaternion.identity);
				}
				StartNextWave();
				// Boss wave
				if (waveNumber % stageData.bossWave == 0)
				{
					StartBossIncoming();
					hasBossSpawned = false;
				}
			}
			yield return null;
		}
	}

	public void StartNextWave()
	{
		// event call
		if (OnEnemyWaveSpawned != null)
			OnEnemyWaveSpawned (waveNumber);
		// Number of enemies spawning curve (used desmos.com for the graph)
		int numEnemies = DifficultyCurveEquation();
		List<StageData.EnemySpawnProperties> prefabPool = stageData.GetSpawnList(waveNumber);
		int i = 0;
		int debugCounter = 0;
		while (i < numEnemies && debugCounter < 1000)
		{
			int randIndex = Random.Range(0, prefabPool.Count);
			StageData.EnemySpawnProperties enemyProp = prefabPool[randIndex];
			if (Random.value < enemyProp.spawnFrequency)
			{
				Vector3 randOpenCell = map.OpenCells[Random.Range(0, map.OpenCells.Count)];
				SpawnEnemy(enemyProp.prefab, randOpenCell);
				i++;
			}
			debugCounter++;
			if (debugCounter > 1000)
			{
				Debug.LogError("Took 1000+ tries to spawn an enemy!!");
			}
		}
		Debug.Log ("Number of enemies in this wave: " + numEnemies);
	}

    private int DifficultyCurveEquation()
    {
		float t = -difficultyCurve + stageData.maxDifficultyVelocity;	// Max slope part of curve (difficulty increases most on this wave)
		float answer = stageData.upperAsymptote / (1 + Mathf.Pow(1.2f, t)) + stageData.lowerAsymptote;
		return Mathf.RoundToInt(answer);
    }

	private void StartBossIncoming()
	{
		OnQueueBossMessage ();
		Invoke ("SpawnBoss", bossSpawnDelay);
	}

	public GameObject SpawnEnemy(GameObject prefab, Vector3 pos)
	{
		GameObject o = Instantiate (prefab);
		o.transform.SetParent (transform);
		if (Random.value < 0.5f)
			o.transform.position = new Vector3 (Random.Range (0, 10), Map.size + 4);
		else
			o.transform.position = new Vector3 (Random.Range (0, 10), -4);

		Enemy e = o.GetComponentInChildren<Enemy> ();
		InitEnemy(e, pos);

		EnemyHealthBar healthBar = enemyHealthBarPool.GetPooledObject ().GetComponent<EnemyHealthBar>();
		healthBar.Init (e);
		healthBar.GetComponent<UIFollow> ().Init(o.transform);
		healthBar.player = player;
		return o;
	}

	public GameObject SpawnEnemyForcePosition(GameObject prefab, Vector3 pos)
	{
		//print (pos);
		GameObject o = Instantiate (prefab);
		o.transform.SetParent (transform);
		o.transform.position = pos;

		Enemy e = o.GetComponentInChildren<Enemy> ();
		e.spawnMethod = Enemy.SpawnMethod.None;     // no spawn animation
		InitEnemy(e, pos);

		EnemyHealthBar healthBar = enemyHealthBarPool.GetPooledObject ().GetComponent<EnemyHealthBar>();
		healthBar.Init (e);
		healthBar.GetComponent<UIFollow> ().Init(o.transform);
		healthBar.player = player;
		return o;
	}

	public void SpawnBoss()
	{
		//SpawnTrappedHeroes();
		hasBossSpawned = true;
		bossSpawn.PlayAnimation ();
		GameObject o = Instantiate (stageData.bossPrefabs [Random.Range (0, stageData.bossPrefabs.Count)]);
		o.transform.SetParent (transform);

		Enemy e = o.GetComponentInChildren<Enemy> ();
		InitEnemy(e, bossSpawn.transform.position);
		e.OnEnemyObjectDisabled += RemoveEnemyFromBossesList;
		BossEnemy boss = (BossEnemy)e;

		bossHealthBar.Init (e);
		bossHealthBar.abilityIconBar.GetComponent<UIFollow> ().Init(o.transform, e.healthBarOffset);
		bosses.Add (boss);
	}

	// public void SpawnTrappedHeroes()
	// {
	// 	List<Vector3> positions = new List<Vector3>();
	// 	float offset = 3f;
	// 	positions.Add(Vector3.right * offset);
	// 	positions.Add(Vector3.left * offset);
	// 	positions.Add(Vector3.up * offset);
	// 	positions.Add(Vector3.down * offset);

	// 	float spawnChance = 0.6f;
	// 	for (int i = 0; i < 4; i ++)
	// 	{
	// 		if (Random.value < spawnChance)
	// 		{
	// 			int randIndex = Random.Range(0, positions.Count);
	// 			SpawnEnemy(trappedHeroPrefab, bossSpawn.transform.position + positions[randIndex]);
	// 			positions.RemoveAt(randIndex);
	// 			spawnChance *= 0.4f;
	// 		}
	// 		else
	// 		{
	// 			break;
	// 		}
	// 	}
	// }

	/* ==========
	 * Helper methods
	 * ==========
	 */
	// Helper method for spawning enemies
	private void InitEnemy(Enemy e, Vector3 spawnPos)
	{
		e.playerTransform = player.transform;
		e.Init(spawnPos, map, level);
		e.OnEnemyDied += IncrementEnemiesKilled;
		e.OnEnemyObjectDisabled += RemoveEnemyFromEnemiesList;
		enemies.Add(e);
	}

	// Counts the current number of alive nemies
	private int NumAliveEnemies()
	{
		int count = 0;
		for (int i = enemies.Count - 1; i >= 0; i --)
		{
			Enemy e = enemies [i];
			if (e.health > 0)
				count++;
		}
		return count;
	}

	// Used as an event listener
	private void RemoveEnemyFromEnemiesList(Enemy e)
	{
		enemies.Remove (e);
	}

	// Used as an event listener
	private void RemoveEnemyFromBossesList(Enemy e)
	{
		bosses.Remove((BossEnemy)e);
	}

	// Used as an event listener
	private void IncrementEnemiesKilled()
	{
		enemiesKilled++;
		if (OnEnemyDefeated != null)
			OnEnemyDefeated();
	}

	public void SetWave(int waveNumber)
	{
		this.waveNumber = waveNumber;
	}

	public void SkipWaveDelay() {
		timeLeftBeforeNextWave = 0;
	}
}
