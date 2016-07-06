using UnityEngine;
using System.Collections;

public class LungingEnemy : Enemy {

	private enum State {
		Moving,
		Attacking
	}
	private State state;
	private bool canAttack = true;

	private bool attacking = false;

	public float chargeTime = 0.5f;
	public float attackTime = 0.2f;
	public float cooldownTime = 1.0f;
	public float lungeSpeed = 10.0f;

	void Start()
	{
		DEFAULT_SPEED = body.moveSpeed;
		StartCoroutine ("MoveState");
	}

	// Update is called once per frame
	void Update () {
		
	}

	/*void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 2f);
	}*/

	protected override IEnumerator MoveState()
	{
		state = State.Moving;
		while (true)
		{
			Vector3 target = (Vector2)(player.position)
				+ new Vector2(Random.Range(-1,2), Random.Range(-1,2));		// add a random offset;

			while (Vector3.Distance(transform.position, target) > 0.1f)
			{
				anim.SetBool ("Moving", true);
				body.Move ((target - transform.position).normalized);

				if (PlayerInRange() && canAttack)
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
		canAttack = false;	

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

		// attack cooldown
		yield return new WaitForSeconds (cooldownTime);
		canAttack = true;
	}

	private void Charge(out Vector3 dir)
	{
		anim.SetTrigger ("Charge");
		body.Move (Vector2.zero);
		dir = (Vector2)(player.position - transform.position); // freeze moving direction
	}

	private void Lunge(Vector3 dir)
	{
		attacking = true;

		anim.SetTrigger ("Attack");

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
				Debug.Log ("Killbox");
			}
		}
	}
}
