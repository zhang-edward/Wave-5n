using UnityEngine;
using System.Collections;

public class DeathSpawnAbility : EnemyAbility {

	private EnemyManager enemyManager;

	public GameObject enemyToSpawn;
	public int numToSpawn;

	public override void Init (Enemy enemy)
	{
		base.Init (enemy);
		enemyManager = EnemyManager.instance;
		enemy.OnEnemyDied += SpawnChildren;
	}

	public void SpawnChildren()
	{
		CreateChild();
		//Debug.Log ("doing this");
		for (int i = 0; i < numToSpawn - 1; i ++) {
			Invoke ("CreateChild", Random.Range (0, 0.5f));
		}
	}

	private void CreateChild()
	{
		enemyManager.SpawnEnemy (enemyToSpawn,
			UtilMethods.RandomOffsetVector2 (enemy.transform.position, 1f));
	}
}
