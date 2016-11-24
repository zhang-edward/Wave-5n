using UnityEngine;
using System.Collections;

public class WalkVicinityState : IMoveState
{
	private enum State {
		Walk,
		Wait
	}
	private State state = State.Walk;

	private Enemy enemy;
	private EntityPhysics body;
	private Transform player;
	private Animator anim;

	private Vector3 target;
	private float waitTimer;	// how long this entity should wait, once it has reached its destination

	public WalkVicinityState(Enemy enemy)
	{
		this.enemy = enemy;
		body = enemy.body;
		player = enemy.player;
		anim = enemy.anim;
		enemy.OnCollideWithMapBorder += ToWaitState;
		ToWalkState ();
	}

	public void UpdateState()
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
		target = (Vector2)(player.position)
			+ new Vector2(Random.Range(-1,2), Random.Range(-1,2));		// add a random offset;

		state = State.Walk;
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
			waitTimer = 1.0f;
			ToWalkState ();
		}
	}
}

