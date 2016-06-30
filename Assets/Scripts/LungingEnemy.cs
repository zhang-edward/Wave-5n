using UnityEngine;
using System.Collections;

public class LungingEnemy : MonoBehaviour {

	public Transform player;
	public EntityPhysics body;
	public Animator anim;

	private float DEFAULT_SPEED;
	private string DEFAULT_STATE;

	private enum State {
		Moving,
		Attacking
	}
	private State state;
	public bool canAttack = true;

	private bool killBox = false;


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

	private IEnumerator MoveState()
	{
		while (state == State.Moving)
		{
			Vector3 target = (Vector2)(player.position)
				+ new Vector2(Random.Range(0,2), Random.Range(0,2));		// add a random offset;

			while (Vector3.Distance(transform.position, target) > 0.1f)
			{
				anim.SetBool ("Moving", true);
				body.move ((target - transform.position).normalized);

				if (isPlayerNearby() && canAttack)
				{
					state = State.Attacking;
					StartCoroutine ("AttackState");
					yield break;
				}
				yield return null;
			}
			body.move (Vector2.zero);
			anim.SetBool ("Moving", false);
			yield return new WaitForSeconds (Random.Range(1, 3));
		}
	}

	private IEnumerator AttackState()
	{
		UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
		//Debug.Log ("attacking: enter");
		canAttack = false;	

		// Charge up before attack
		Vector3 dir;
		charge(out dir);
		yield return new WaitForSeconds (0.5f);

		// Lunge
		lunge(dir);
		yield return new WaitForSeconds (0.2f);

		// Reset vars
		killBox = false;
		body.moveSpeed = DEFAULT_SPEED;
		body.move (dir.normalized);
		yield return new WaitForSeconds (0.3f);

		state = State.Moving;
		StartCoroutine ("MoveState");

		// attack cooldown
		yield return new WaitForSeconds (1.0f);
		canAttack = true;
	}

	private void charge(out Vector3 dir)
	{
		anim.SetTrigger ("Charge");
		body.move (Vector2.zero);
		dir = (Vector2)(player.position - transform.position) // freeze moving direction
			+ new Vector2(Random.value, Random.value);		// add a random offset
	}

	private void lunge(Vector3 dir)
	{
		killBox = true;

		anim.SetTrigger ("Attack");
		body.moveSpeed = 10f;
		body.move (dir.normalized);
	}

	private bool isPlayerNearby()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 2f);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag ("Player"))
			{
				return true;
			}
		}
		return false;
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			if (killBox)
			{
				Player player = col.GetComponentInChildren<Player>();
				Debug.Log ("Killbox");
			}
		}
	}
}
