using UnityEngine;
using System.Collections;

public class FleeState : MoveState
{
	private Vector3 dir;
	private Map map;
	private bool updatingDir;
	private float randAngle = 30f;

	public override void Init(Enemy e, Transform player)
	{
		base.Init(e, player);
		anim = e.anim;
		map = e.map;
	}

	public override void UpdateState()
	{
		dir = (transform.position - player.transform.position).normalized;
		if (map.WithinOpenCells(transform.position + dir)) {
			body.Move(dir);
			if (!anim.player.IsPlayingAnimation(moveState))
				anim.Play(moveState);
		}
	}
}