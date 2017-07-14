using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	private const float DIFFICULTY_CURVE = 5f;

	[Header("Set from Scene Hierarchy")]
	public Player player;
	public int enemiesKilled { get; private set; }
	public Map map;

	public ObjectPooler enemyHealthBarPool;

	private List<Enemy> enemies = new List<Enemy>();
	public List<Enemy> Enemies { get {return enemies;} }

	private int level;
	public StageData stageData { get; private set; }

	public EnemyHealthBar bossHealthBar;
	private BossSpawn bossSpawn;
	private float bossSpawnDelay = 8f;
	private bool hasBossSpawned = true;

	[Header("Pickups and Prefabs")]
	public GameObject heartPickup;
	public GameObject upgradePickup;
	public GameObject moneyPickup;
	public GameObject soulPickup;
	public GameObject trappedHeroPrefab;

	public int waveNumber { get; private set; }
	private int difficultyCurve = 0;	// number to determine the number of enemies to spawn
	//public ShopNPC shopNPC;

	public List<BossEnemy> bosses;

	public delegate void EnemyWaveSpawned (int waveNumber);
	public event EnemyWaveSpawned OnEnemyWaveSpawned;
	public delegate void EnemyWaveCompleted ();
	public event EnemyWaveCompleted OnEnemyWaveCompleted;
	public delegate void BossIncoming();
	public event BossIncoming OnQueueBossMessage;

	public void Init(StageData data)
	{
		this.stageData = data;
		this.level = data.level;
		bossSpawn = map.bossSpawn.GetComponent<BossSpawn> ();
		StartCoroutine (StartSpawningEnemies ());
	}

	private IEnumerator StartSpawningEnemies()
	{
		// Wait for GUIManager to display its first message
		yield return new WaitForSeconds(3f);
		for (;;)
		{
			// all enemies dead
			if (NumAliveEnemies() <= 0 && hasBossSpawned)
			{
				if (waveNumber >= 1)
				{
					OnEnemyWaveCompleted ();
				}
				waveNumber++;
				difficultyCurve++;
				// spawn shop every 3 waves after wave 5
				if (waveNumber % stageData.shopWave == 0 && waveNumber > 1)
				{
					if (player.hero.powerUpManager.GetNumUpgradesLeft() > 0)
						Instantiate(upgradePickup, map.CenterPosition, Quaternion.identity);
					// spawn shop npc before the boss
					// wait for shopNPC to disappear
					/*while (shopNPC.gameObject.activeInHierarchy)
						yield return null;*/
				}
				// if it is the wave after a boss wave (just defeated boss), spawn heart pickup
				if (waveNumber % stageData.bossWave == 1 && waveNumber != 1)
				{
					Instantiate(heartPickup, map.CenterPosition, Quaternion.identity);
				}
				StartNextWave();
				// every 'bossWave' waves, spawn a boss
				if (waveNumber % stageData.bossWave == 0)
				{
					StartBossIncoming();
					hasBossSpawned = false;
					level++;
				}
			}
			yield return null;
		}
	}

	private void StartNextWave()
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
		float t = -difficultyCurve + 18;	// 18 = max slope part of curve (difficulty increases most on this wave)
		float answer = 20 / (1 + Mathf.Pow(1.2f, t)) + 5;
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
		SpawnTrappedHeroes();
		hasBossSpawned = true;
		bossSpawn.PlayAnimation ();
		GameObject o = Instantiate (stageData.bossPrefabs [Random.Range (0, stageData.bossPrefabs.Count)]);
		o.transform.SetParent (transform);

		Enemy e = o.GetComponentInChildren<Enemy> ();
		InitEnemy(e, bossSpawn.transform.position);
		e.OnEnemyObjectDisabled += RemoveEnemyFromBossesList;
		BossEnemy boss = (BossEnemy)e;
		boss.soulPickupPrefab = soulPickup;

		bossHealthBar.Init (e);
		bossHealthBar.abilityIconBar.GetComponent<UIFollow> ().Init(o.transform, e.healthBarOffset);
		bosses.Add (boss);
	}

	public void SpawnTrappedHeroes()
	{
		List<Vector3> positions = new List<Vector3>();
		float offset = 3f;
		positions.Add(Vector3.right * offset);
		positions.Add(Vector3.left * offset);
		positions.Add(Vector3.up * offset);
		positions.Add(Vector3.down * offset);

		float spawnChance = 0.6f;
		for (int i = 0; i < 4; i ++)
		{
			if (Random.value < spawnChance)
			{
				int randIndex = Random.Range(0, positions.Count);
				SpawnEnemy(trappedHeroPrefab, bossSpawn.transform.position + positions[randIndex]);
				positions.RemoveAt(randIndex);
				spawnChance *= 0.4f;
			}
			else
			{
				break;
			}
		}
	}

	public bool IsStageComplete()
	{
		return waveNumber > stageData.goalWave; 
	}

	/* ==========
	 * Helper methods
	 * ==========
	 */

	// Helper method for spawning enemies
	private void InitEnemy(Enemy e, Vector3 spawnPos)
	{
		e.player = player.transform;
		e.moneyPickupPrefab = moneyPickup;
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
	}
}
