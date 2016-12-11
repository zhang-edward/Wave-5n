using UnityEngine;
using System.Collections;

public class TankAbility : EnemyAbility
{
	public override void Init (Enemy enemy)
	{
		base.Init (enemy);
		enemy.OnEnemyInit += OnInit;
	}

	private void OnInit()
	{
		enemy.maxHealth = (int)((float)enemy.maxHealth * 2f);
		enemy.body.moveSpeed /= 2f;
	}
}

