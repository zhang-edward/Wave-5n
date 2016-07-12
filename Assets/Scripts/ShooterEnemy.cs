﻿using UnityEngine;
using System.Collections;

public class ShooterEnemy : Enemy {

	private enum State {
		Moving,
		Attacking
	}
	private State state;
	public float attackTimer;

	public GameObject projectile;

	public float chargeTime = 0.5f;
	public float attackTime = 0.2f;
	public float cooldownTime = 1.0f;

	// Update is called once per frame
	void Update () {
		if (attackTimer > 0)
			attackTimer -= Time.deltaTime;
	}

	/*void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 2f);
	}*/

	protected override IEnumerator MoveState()
	{
		UnityEngine.Assertions.Assert.IsTrue (body.moveSpeed == DEFAULT_SPEED);
		state = State.Moving;
		while (true)
		{
			Vector3 target = (Vector2)(player.position);
				//+ new Vector2(Random.Range(-3,4), Random.Range(-3,4));		// add a random offset;

			Vector3 oldPos = Vector3.zero;	// track transform velocity to check if stuck on a wall
			float t = 0;
			while (Vector3.Distance(transform.position, target) > 0.1f)
			{
				// Check if stuck on a wall
				Vector3 velocity = (transform.position - oldPos) / Time.deltaTime;
				if (velocity.magnitude < Mathf.Epsilon)
				{
					t += Time.deltaTime;
					if (t > 0.05f)
						break;
				}
				else
					t = 0;
				oldPos = transform.position;

				anim.SetBool ("Moving", true);
				body.Move ((target - transform.position).normalized);

				if (PlayerInRange() && attackTimer <= 0)
				{
					StartCoroutine ("AttackState");
					yield break;
				}
				yield return null;
			}
			body.Move (Vector2.zero);
			anim.SetBool ("Moving", false);
			yield return new WaitForSeconds (1.0f);
		}
	}

	private IEnumerator AttackState()
	{
		//UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
		//Debug.Log ("attacking: enter");
		state = State.Attacking;
		attackTimer = cooldownTime;	

		// Charge up before attack
		Vector3 dir;
		Charge(out dir);
		yield return new WaitForSeconds (chargeTime);

		// Shoot
		Shoot(dir.normalized);
		yield return new WaitForSeconds (attackTime);

		state = State.Moving;
		StartCoroutine ("MoveState");
	}

	protected override void ResetVars ()
	{
		body.gameObject.layer = DEFAULT_LAYER;
		body.moveSpeed = DEFAULT_SPEED;
	}

	private void Charge(out Vector3 dir)
	{
		anim.SetTrigger ("Charge");
		body.Move (Vector2.zero);
		dir = (Vector2)(player.position - transform.position); // freeze moving direction
			//+ new Vector2(Random.value, Random.value);		// add a random offset
	}

	private void Shoot(Vector2 dir)
	{
		anim.SetTrigger ("Attack");
		GameObject o = Instantiate (projectile);
		Projectile p = o.GetComponent<Projectile> ();
		UnityEngine.Assertions.Assert.IsNotNull (p);
		p.Init (transform.position, dir, 5, "Player", 1);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
	}
}
