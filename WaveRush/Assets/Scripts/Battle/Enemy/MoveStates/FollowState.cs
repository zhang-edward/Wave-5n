﻿using UnityEngine;
using System.Collections;

public class FollowState : MoveState
{
	private Vector3 target;
	private Animator anim;

	public override void Init(Enemy e, Transform player)
	{
		base.Init(e, player);
		anim = e.anim;
	}

	public override void UpdateState()
	{
		target = Vector3.Lerp (target, (player.position - enemy.transform.position).normalized, 0.1f);
		body.Move (target);
	}
}

