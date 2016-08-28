using UnityEngine;
using System.Collections;

public class FollowState : IMoveState
{
	private Enemy enemy;
	private EntityPhysics body;
	private Transform player;

	public FollowState(Enemy enemy)
	{
		this.enemy = enemy;
		body = enemy.body;
		player = enemy.player;
	}

	public void UpdateState()
	{
		body.Move ((player.position - enemy.transform.position).normalized);
	}
}

