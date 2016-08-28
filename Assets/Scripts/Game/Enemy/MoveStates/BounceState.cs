using UnityEngine;
using System.Collections;

public class BounceState : IMoveState
{
	private Enemy enemy;
	private EntityPhysics body;
	private Transform player;

	public Vector2 dir;

	public BounceState(Enemy enemy)
	{
		this.enemy = enemy;
		body = enemy.body;
		player = enemy.player;
		enemy.OnCollideWithMapBorder += Bounce;
		Bounce ();
	}

	public void UpdateState()
	{
		body.Move (dir);
	}

	private void Bounce()
	{
		dir = (player.position - enemy.transform.position).normalized;
	}
}

