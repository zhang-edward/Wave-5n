using UnityEngine;
using System.Collections;

public class WalkVicinityState : MoveState
{
	private enum State {
		Walk,
		Wait
	}
	private State state = State.Walk;

	private Animator anim;

	private Vector3 target;
	private float waitTimer;	// how long this entity should wait, once it has reached its destination

	public float waitTime = 1.0f;
	public float walkRadius = 1.0f;

	public override void Init (Enemy e, EntityPhysics body, Transform player)
	{
		base.Init (e, body, player);
		anim = e.anim;
	}

	public override void Reset()
	{
		ToWalkState ();
	}

	// Simple FSM
	public override void UpdateState()
	{
		switch (state)
		{
		case State.Walk:
			WalkState ();
			break;
		case State.Wait:
			WaitState ();
			break;
		}
	}

	private void ToWalkState()
	{
		target = (Vector2)(player.position) + new Vector2(
				Random.Range(-walkRadius, walkRadius),
				Random.Range(-walkRadius, walkRadius));		// add a random offset;
		state = State.Walk;
		print("moving");
	}

	private void WalkState()
	{
		if (Vector3.Distance (enemy.transform.position, target) > 0.1f)
		{
			// move
			anim.SetBool ("Moving", true);
			body.Move ((target - enemy.transform.position).normalized);
		}
		else
		{
			ToWaitState ();
		}
	}

	private void ToWaitState()
	{
//		Debug.Log ("WaitState");

		body.Move (Vector2.zero);
		anim.SetBool ("Moving", false);

		state = State.Wait;
	}

	private void WaitState()
	{
		waitTimer-= Time.deltaTime;
		if (waitTimer <= 0)
		{
			waitTimer = waitTime;
			ToWalkState ();
		}
	}
}

