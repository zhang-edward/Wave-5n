using UnityEngine;
using System.Collections;

public class LungingEnemy : Enemy {

	private enum State {
		Moving,
		Attacking
	}
	private State state;
	private float attackTimer;

	private bool attacking = false;

	[Header("LungingEnemy Properties")]
	public float chargeTime = 0.5f;
	public float attackTime = 0.2f;
	public float cooldownTime = 1.0f;
	public float lungeSpeed = 10.0f;

	[Header("Audio")]
	public AudioClip swingSound;

	// Update is called once per frame
	void Update () {
		if (attackTimer > 0)
			attackTimer -= Time.deltaTime;
	}

	public override void Damage (int amt)
	{
		base.Damage (amt);
		attackTimer += 0.5f;
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
			Vector3 target = (Vector2)(player.position)
				+ new Vector2(Random.Range(-1,2), Random.Range(-1,2));		// add a random offset;

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

		// Lunge
		Lunge(dir);
		yield return new WaitForSeconds (attackTime);

		// Reset vars
		attacking = false;
		body.moveSpeed = DEFAULT_SPEED;
		body.Move (dir.normalized);
		yield return new WaitForSeconds (0.3f);

		StartCoroutine ("MoveState");
	}

	protected override void ResetVars()
	{
		body.gameObject.layer = DEFAULT_LAYER;
		body.moveSpeed = DEFAULT_SPEED;
		attacking = false;
	}

	private void Charge(out Vector3 dir)
	{
		//anim.ResetTrigger ("Charge");
		anim.SetTrigger ("Charge");
		body.Move (Vector2.zero);
		dir = (Vector2)(player.position - transform.position); // freeze moving direction
	}

	private void Lunge(Vector3 dir)
	{
		attacking = true;

		anim.SetTrigger ("Attack");
		SoundManager.instance.RandomizeSFX (swingSound);

		body.moveSpeed = lungeSpeed;
		body.Move (dir.normalized);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			if (attacking)
			{
				Player player = col.GetComponentInChildren<Player>();
				player.Damage (1);
			}
		}
	}
}
