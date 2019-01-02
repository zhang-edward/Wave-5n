using UnityEngine;
using System.Collections;

public class FollowState : MoveState
{
	public float minDistance = 1f; 
	private Vector3 target;

	public override void Init(Enemy e, Transform player) {
		base.Init(e, player);
		anim = e.anim;
	}

	public override void UpdateState() {
		if (Vector3.Distance(enemy.transform.position, player.position) > minDistance) {
			target = Vector3.Lerp (target, (player.position - enemy.transform.position).normalized, 0.1f);
			if (!anim.player.IsPlayingAnimation(moveState))
				anim.Play (moveState);
			body.Move (target);
		}
		else
			anim.player.ResetToDefault();
	}
}

