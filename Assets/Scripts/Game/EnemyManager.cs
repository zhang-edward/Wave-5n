﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyManager : MonoBehaviour {

	private const float DIFFICULTY_CURVE = 3.5f;

	public Player player;
	public Map map;

	public ObjectPooler enemyHealthBarPool;

	private List<Enemy> enemies = new List<Enemy>();
	public EnemyManagerInfo info;

	public EnemyHealthBar bossHealthBar;
	private BossSpawn bossSpawn;
	private int waveNumber = 0;

	public delegate void EnemyWaveSpawned (int waveNumber);
	public event EnemyWaveSpawned OnEnemyWaveSpawned;

	void Start()
	{
		bossSpawn = map.bossSpawn.GetComponent<BossSpawn> ();
		player.OnPlayerInitialized += InitSpawnEnemies;
	}

	private void InitSpawnEnemies()
	{
		StartCoroutine (StartSpawningEnemies ());
	}

	private IEnumerator StartSpawningEnemies()
	{
		while (true)
		{
			if (NumAliveEnemies() < 3)
			{
				waveNumber++;
				int numToSpawn = Mathf.RoundToInt (DIFFICULTY_CURVE * Mathf.Log (waveNumber) + 5);
				for (int i = 0; i < numToSpawn; i++)
					SpawnEnemy ();
				// every 5 waves, spawn a boss
				if (waveNumber % 5 == 0)
					SpawnBoss ();
				if (OnEnemyWaveSpawned != null)
					OnEnemyWaveSpawned (waveNumber);

				Debug.Log ("Number of enemies in this wave: " + numToSpawn);
			}
			yield return null;
		}
	}

	public void SpawnEnemy()
	{
		Vector3 randOpenCell = (Vector3)map.OpenCells [Random.Range (0, map.OpenCells.Count)];
		GameObject o = Instantiate (info.enemyPrefabs [Random.Range (0, info.enemyPrefabs.Length)]);
		o.transform.SetParent (transform);
		if (Random.value < 0.5f)
			o.transform.position = new Vector3 (Random.Range (0, 10), Map.size + 4);
		else
			o.transform.position = new Vector3 (Random.Range (0, 10), -4);

		Enemy e = o.GetComponentInChildren<Enemy> ();
		EnemyHealthBar healthBar = enemyHealthBarPool.GetPooledObject ().GetComponent<EnemyHealthBar>();
		healthBar.Init (e);
		healthBar.player = player;

		e.Init (randOpenCell);
		e.player = player.transform;
		enemies.Add (e);
	}

	public void SpawnBoss()
	{
		bossSpawn.PlayAnimation ();
		GameObject o = Instantiate (info.bossPrefabs [Random.Range (0, info.bossPrefabs.Length)]);
		o.transform.SetParent (transform);
		Enemy e = o.GetComponentInChildren<Enemy> ();
		e.Init (bossSpawn.transform.position);
		e.player = player.transform;
		bossHealthBar.Init (e);
		//enemies.Add (e);
	}

	private int NumAliveEnemies()
	{
		int count = 0;
		foreach(Enemy e in enemies)
		{
			if (e.health > 0)
				count++;
		}
		return count;
	}
}
