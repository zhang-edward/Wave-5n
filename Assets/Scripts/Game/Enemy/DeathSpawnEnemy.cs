using UnityEngine;
using System.Collections;

public class DeathSpawnEnemy : Enemy {

	private EnemyManager enemyManager;

	public GameObject enemyToSpawn;
	public int numToSpawn;
	public int damage = 1;

	public override void Init (Vector3 spawnLocation, Map map)
	{
		base.Init (spawnLocation, map);
		enemyManager = transform.GetComponentInParent<EnemyManager> ();
	}

	protected override IEnumerator MoveState()
	{
		moveState = new FollowState (this);
		while (true)
		{
			moveState.UpdateState ();
			yield return null;
		}
	}

	protected override void ResetVars ()
	{
		body.gameObject.layer = DEFAULT_LAYER;
		body.moveSpeed = DEFAULT_SPEED;
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			Player player = col.GetComponentInChildren<Player>();
			if (health > 0 && !hitDisabled)
				player.Damage (damage);
		}
	}

	public override void Die ()
	{
		base.Die ();
		SpawnChildren ();
	}

	public void SpawnChildren()
	{
		for (int i = 0; i < numToSpawn; i ++)
		{
			Invoke ("CreateChild", Random.Range (0, 0.5f));
		}
	}

	private void CreateChild()
	{
		enemyManager.SpawnEnemy (enemyToSpawn,
			UtilMethods.RandomOffsetVector2 (transform.position, 1f));
	}
}
