using UnityEngine;
using System.Collections;

public class SpeedAbility : EnemyAbility
{
	public override void Init (Enemy enemy)
	{
		base.Init (enemy);
		enemy.OnEnemyInit += OnInit;
	}

	private void OnInit()
	{
		enemy.body.moveSpeed *= 3f;
	}
}

