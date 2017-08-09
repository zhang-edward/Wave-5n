﻿using UnityEngine;
using System.Collections;

public class WalkRangeState : MoveState
{
	private enum State
	{
		Walk,
		Wait
	}
	private State state = State.Walk;
	private Animator anim;
	private Vector3 target;
	private float waitTimer;    // how long this entity should wait, once it has reached its destination
	private Map map;

	public float waitTime = 1.0f;
	public float minDistance = 4.0f;
	public float maxDistance = 5.0f;

	public override void Init(Enemy e, Transform player)
	{
		base.Init(e, player);
		anim = e.anim;
		map = e.map;
	}

	public override void Reset()
	{
		ToWalkState();
	}

	// Simple FSM
	public override void UpdateState()
	{
		switch (state)
		{
			case State.Walk:
				WalkState();
				break;
			case State.Wait:
				WaitState();
				break;
		}
	}

	private void ToWalkState()
	{
		bool targetWithinMap = false;
		int debugCounter = 0;
		while (!targetWithinMap && debugCounter < 20)
		{
			Vector3 offset = new Vector3(
				Random.Range(minDistance, maxDistance) * UtilMethods.RandSign(),
				Random.Range(minDistance, maxDistance) * UtilMethods.RandSign());
			Vector3.ClampMagnitude(offset, maxDistance);
			target = player.transform.position + offset;
			targetWithinMap = map.WithinOpenCells(target);
			debugCounter++;
		}
		if (debugCounter >= 20)
		{
			Debug.LogError("Took 20+ tries to find a position!");
		}
		state = State.Walk;
	}

	private void WalkState()
	{
		if (Vector3.Distance(enemy.transform.position, target) > 0.1f)
		{
			// move
			anim.SetBool("Moving", true);
			body.Move((target - enemy.transform.position).normalized);
		}
		else
		{
			ToWaitState();
		}
	}

	private void ToWaitState()
	{
		// Debug.Log ("WaitState");

		body.Move(Vector2.zero);
		anim.SetBool("Moving", false);

		state = State.Wait;
	}

	private void WaitState()
	{
		waitTimer -= Time.deltaTime;
		if (waitTimer <= 0)
		{
			waitTimer = waitTime;
			ToWalkState();
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, minDistance);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(transform.position, maxDistance);
	}
}

