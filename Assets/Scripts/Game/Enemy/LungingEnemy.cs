using UnityEngine;
using System.Collections;

public class LungingEnemy : Enemy {

	private float attackTimer;

	private bool attacking = false;

	[Header("LungingEnemy Properties")]
	public float chargeTime = 0.5f;
	public float attackTime = 0.2f;
	public float cooldownTime = 1.0f;
	public float lungeSpeed = 10.0f;
	[Space]
	public int damage;

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

	protected override IEnumerator MoveState()
	{
		//UnityEngine.Assertions.Assert.IsTrue (body.moveSpeed == DEFAULT_SPEED);
		moveState = GetAssignedMoveState ();
		while (true)
		{
			moveState.UpdateState ();
			if (PlayerInRange() && attackTimer <= 0)
			{
				StartCoroutine ("AttackState");
				yield break;
			}
			yield return null;
		}
	}

	private IEnumerator AttackState()
	{
		//UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
		//Debug.Log ("attacking: enter");
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
		anim.CrossFade ("charge", 0f);		// triggers are unreliable, crossfade forces state to execute
		body.Move (Vector2.zero);
		dir = (Vector2)(player.position - transform.position); // freeze moving direction
	}

	private void Lunge(Vector3 dir)
	{
		attacking = true;

		anim.CrossFade ("attack", 0f);		// triggers are unreliable, crossfade forces state to execute
		SoundManager.instance.RandomizeSFX (swingSound);

		body.moveSpeed = lungeSpeed;
		body.Move (dir.normalized);
	}

	protected override void OnTriggerEnter2D(Collider2D col)
	{
		base.OnTriggerEnter2D (col);
		if (col.CompareTag("Player"))
		{
			if (attacking)
			{
				Player player = col.GetComponentInChildren<Player>();
				player.Damage (damage);
			}
		}
	}
}
