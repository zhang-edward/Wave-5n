using UnityEngine;
using System.Collections;

public class DeathSpawnAbility : EnemyAbility {

	private EnemyManager enemyManager;

	public GameObject enemyToSpawn;
	public int numToSpawn;

	public override void Init (Enemy enemy)
	{
		base.Init (enemy);
		enemyManager = transform.GetComponentInParent<EnemyManager> ();
		enemy.OnEnemyDied += SpawnChildren;
	}

	public void SpawnChildren()
	{
		//Debug.Log ("doing this");
		for (int i = 0; i < numToSpawn; i ++)
		{
			Invoke ("CreateChild", Random.Range (0, 0.5f));
		}
	}

	private void CreateChild()
	{
		enemyManager.SpawnEnemy (enemyToSpawn,
			UtilMethods.RandomOffsetVector2 (enemy.transform.position, 1f));
	}
}
