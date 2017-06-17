using UnityEngine;
using System.Collections;

public class FollowState : MoveState
{
	private Vector3 target;

	public override void UpdateState()
	{
		target = Vector3.Lerp (target, (player.position - enemy.transform.position).normalized, 0.1f);
		body.Move (target);
	}
}

