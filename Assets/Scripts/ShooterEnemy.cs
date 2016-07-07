using UnityEngine;
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
		state = State.Moving;
		while (true)
		{
			//Debug.Log ("hello world");
			Vector3 target = (Vector2)(player.position)
				+ new Vector2(Random.Range(-3,4), Random.Range(-3,4));		// add a random offset;

			while (Vector3.Distance(transform.position, target) > 0.1f)
			{
				anim.SetBool ("Moving", true);
				//Debug.Log ("Hello");
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
			yield return new WaitForSeconds (Random.Range(1, 3));
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
		body.moveSpeed = DEFAULT_SPEED;
	}

	private void Charge(out Vector3 dir)
	{
		anim.SetTrigger ("Charge");
		body.Move (Vector2.zero);
		dir = (Vector2)(player.position - transform.position) // freeze moving direction
			+ new Vector2(Random.value, Random.value);		// add a random offset
	}

	private void Shoot(Vector2 dir)
	{
		anim.SetTrigger ("Attack");
		GameObject o = Instantiate (projectile);
		Projectile p = o.GetComponent<Projectile> ();
		UnityEngine.Assertions.Assert.IsNotNull (p);
		p.Init (transform.position, dir);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
	}
}
