using UnityEngine;
using System.Collections;

public class DwarfAbility : EnemyAbility
{
	public override void Init (Enemy enemy)
	{
		base.Init (enemy);
		enemy.OnEnemyInit += OnInit;
	}

	private void OnInit()
	{
		enemy.maxHealth = (int)((float)enemy.maxHealth * 0.5f);
		if (enemy.maxHealth <= 0)
			enemy.maxHealth = 1;
		enemy.body.moveSpeed *= 1.1f;
		enemy.transform.parent.localScale = Vector3.one * 0.8f;
	}
}

