using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	public static EnemyManager instance;
	public const float WAVE_SPAWN_DELAY = 10.0f;

	[Header("Set from Scene Hierarchy")]
	public Player player;
	public int enemiesKilled { get; private set; }
	public Map map;

	public ObjectPooler enemyHealthBarPool;

	private List<Enemy> activeEnemies = new List<Enemy>();
	public List<Enemy> Enemies { get {return activeEnemies;} }

	public int level;
	public StageData stageData { get; private set; }

	public EnemyHealthBar bossHealthBar;
	private float bossSpawnDelay = 0f;
	private bool hasBossSpawned = true;

	[Header("Pickups and Prefabs")]
	public GameObject heartPickup;
	public GameObject upgradePickup;
	public GameObject trappedHeroPrefab;
	public GameObject endPortalPrefab;
	[Header("EnemyManager Properties")]
	public bool paused;
	public int waveNumber { get; private set; }
	private int difficultyCurve = 0;	// number to determine the number of enemies to spawn
	[Header("Effects")]
	public SimpleAnimation bossDeathEffect;

	// Hidden properties
	[HideInInspector] public Queue<GameObject> enemyQueue = new Queue<GameObject>();
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

	void Awake() {
		if (instance == null)
			instance = this;
		else
			Destroy(this.gameObject);
	}

	public void Init(StageData data)
	{
		this.stageData = data;
		this.level = data.levelRaw;
		UnityEngine.Assertions.Assert.IsTrue(level > 0);
		GenerateNewEnemyQueue(1);
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
			// All enemies dead (including boss)
			if (NumAliveEnemies() <= 0 && hasBossSpawned) {
				// Only check if wave is completed if it's not the first wave
				if (waveNumber >= 1) {
					// If the stage is completed
					if (waveNumber % stageData.goalWave == 0) {
						// Event call
						if (OnStageCompleted != null)
							OnStageCompleted();
						// Pause before continuing (though the game should never get to this point)
						paused = true;
						while (paused)
							yield return null;
					}
					// If the stage is not completed
					else {
						// Event call
						GenerateNewEnemyQueue(waveNumber + 1);
						if (OnEnemyWaveCompleted != null)
							OnEnemyWaveCompleted();
						// Delay before next wave
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
				StartCoroutine(SpawnNextWave());
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

	private void GenerateNewEnemyQueue(int wave) {
		enemyQueue.Clear();

		// Get number of enemies total to spawn
		int numEnemies;
		StageData.WaveProperties waveProperties = stageData.GetWaveProperties(wave);
		if (waveProperties.waveNumber == 0)
			numEnemies = Mathf.CeilToInt(Mathf.Lerp(stageData.lowerLimit, stageData.upperLimit, (float)((wave - 1) % stageData.goalWave) / stageData.goalWave));
		else
			numEnemies = waveProperties.numEnemies;
		
		// Get enemy pools for spawning
		List<StageData.EnemySpawnProperties> randomEnemyPool = stageData.GetRandomEnemyPool(wave);
		List<StageData.WaveProperties.WaveSpecificEnemyProperties> specificEnemyPool = stageData.GetSpecificEnemyPool(wave);

		// Enemy pools used to build the queue
		List<GameObject> spawnFirst = new List<GameObject>();
		List<GameObject> spawnLast = new List<GameObject>();
		List<GameObject> randomPool = new List<GameObject>();

		foreach (StageData.WaveProperties.WaveSpecificEnemyProperties props in specificEnemyPool) {
			switch (props.spawnMode) {
				case StageData.WaveProperties.EnemySpawnMode.Beginning:
					for (int i = 0; i < props.numEnemies; i ++) 
						spawnFirst.Add(props.prefab);
					break;
				case StageData.WaveProperties.EnemySpawnMode.End:
					for (int i = 0; i < props.numEnemies; i ++) 
						spawnLast.Add(props.prefab);
					break;
				case StageData.WaveProperties.EnemySpawnMode.Random:
					for (int i = 0; i < props.numEnemies; i ++) 
						randomPool.Add(props.prefab);
					break;	
			}
		}
		
		int numEnemiesRemaining = numEnemies - (spawnFirst.Count + spawnLast.Count + randomPool.Count);
		while (numEnemiesRemaining > 0) {
			int randIndex = Random.Range(0, randomEnemyPool.Count);
			StageData.EnemySpawnProperties enemyProp = randomEnemyPool[randIndex];
			if (Random.value < enemyProp.spawnFrequency) {
				randomPool.Add(enemyProp.prefab);
				numEnemiesRemaining --;
			}
		}

		spawnFirst.Shuffle();
		randomPool.Shuffle();
		spawnLast.Shuffle();
		foreach (GameObject o in spawnFirst) 
			enemyQueue.Enqueue(o);
		foreach (GameObject o in randomPool)
			enemyQueue.Enqueue(o);
		foreach (GameObject o in spawnLast)
			enemyQueue.Enqueue(o);

		print ("Number of enemies in this wave: " + numEnemies);
	}

	public IEnumerator SpawnNextWave() {
		// Event call
		if (OnEnemyWaveSpawned != null)
			OnEnemyWaveSpawned (waveNumber);

		StageData.WaveProperties waveProperties = stageData.GetWaveProperties(waveNumber);

		while (enemyQueue.Count > 0) {
			while (activeEnemies.Count < waveProperties.maxNumActiveEnemies) {
				SpawnEnemy(enemyQueue.Dequeue(), map.OpenCells[Random.Range(0, map.OpenCells.Count)]);
				yield return null;
			}
			yield return null;
		}
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
		o.transform.position = GenerateEnemyOutOfBoundsSpawnPos();

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
		// bossSpawn.PlayAnimation ();
		GameObject o = Instantiate (stageData.bossPrefabs [Random.Range (0, stageData.bossPrefabs.Count)]);
		o.transform.SetParent(transform);

		Enemy e = o.GetComponentInChildren<Enemy> ();
		InitEnemy(e, map.CenterPosition);
		e.OnEnemyObjectDisabled += RemoveEnemyFromBossesList;
		BossEnemy boss = (BossEnemy)e;

		bossHealthBar.Init (e);
		bossHealthBar.abilityIconBar.GetComponent<UIFollow> ().Init(o.transform, e.healthBarPos);
		bosses.Add (boss);
	}

#region Helper Methods
	private Vector3 GenerateEnemyOutOfBoundsSpawnPos() {
		Vector3 answer;
		if (Random.value < 0.5f) {
			// Top/Bottom spawn
			if (Random.value < 0.5f)
				answer = new Vector3 (Random.Range (0, 10), Map.size + 4);
			else
				answer = new Vector3 (Random.Range (0, 10), -4);
		}
		else {
			// Left/Right spawn
			if (Random.value < 0.5f)
				answer = new Vector3(-4, Random.Range(0, 10));
			else
				answer = new Vector3(Map.size + 4, Random.Range(0, 10));
		}
		return answer;
	}

	// Helper method for spawning enemies
	private void InitEnemy(Enemy e, Vector3 spawnPos) {
		e.playerTransform = player.transform;
		e.Init(spawnPos, map, level);
		e.OnEnemyDied += IncrementEnemiesKilled;
		e.OnEnemyObjectDisabled += RemoveEnemyFromEnemiesList;
		activeEnemies.Add(e);
	}

	// Counts the current number of alive nemies
	private int NumAliveEnemies() {
		int count = 0;
		for (int i = activeEnemies.Count - 1; i >= 0; i --) {
			Enemy e = activeEnemies [i];
			if (e.health > 0)
				count++;
		}
		return count;
	}

	// Used as an event listener
	private void RemoveEnemyFromEnemiesList(Enemy e) {
		activeEnemies.Remove (e);
	}

	// Used as an event listener
	private void RemoveEnemyFromBossesList(Enemy e) {
		bosses.Remove((BossEnemy)e);
	}

	// Used as an event listener
	private void IncrementEnemiesKilled() {
		enemiesKilled++;
		if (OnEnemyDefeated != null)
			OnEnemyDefeated();
	}

	public void SetWave(int waveNumber) {
		this.waveNumber = waveNumber;
	}

	public void SkipWaveDelay() {
		timeLeftBeforeNextWave = 0;
	}
#endregion
}
