using UnityEngine;
using System.Collections;

public class BounceState : MoveState
{
	private Vector2 dir;

	public BounceState(Enemy enemy)
	{
		this.enemy = enemy;
		body = enemy.body;
		player = enemy.playerTransform;
		enemy.OnCollideWithMapBorder += Bounce;
		Bounce ();
	}

	public override void UpdateState()
	{
		body.Move (dir);
	}

	private void Bounce()
	{
		dir = (player.position - enemy.transform.position).normalized;
	}
}

