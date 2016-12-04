using UnityEngine;
using System.Collections;

public class FollowState : IMoveState
{
	private Enemy enemy;
	private EntityPhysics body;
	private Transform player;

	private Vector3 target;

	public FollowState(Enemy enemy)
	{
		this.enemy = enemy;
		body = enemy.body;
		player = enemy.player;
	}

	public void UpdateState()
	{
		target = Vector3.Lerp (target, (player.position - enemy.transform.position).normalized, 0.1f);
		body.Move (target);
	}
}

