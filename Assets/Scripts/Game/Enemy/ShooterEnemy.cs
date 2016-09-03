using UnityEngine;
using System.Collections;

public class ShooterEnemy : Enemy {

	[Header("ShooterEnemy Properties")]
	public float attackTimer;

	private ObjectPooler projectilePool;
	public Sprite projectileSprite;
	public Transform shootPoint;

	public float projectileSpeed = 5;
	public float chargeTime = 0.5f;
	public float attackTime = 0.2f;
	public float cooldownTime = 1.0f;

	[Header("Audio")]
	public AudioClip shootSound;

	public override void Init(Vector3 spawnLocation, Map map)
	{
		base.Init (spawnLocation, map);
		projectilePool = ObjectPooler.GetObjectPooler ("Projectile");
	}

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
		moveState = GetAssignedMoveState();
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

		// Shoot
		Shoot(dir.normalized);
		yield return new WaitForSeconds (attackTime);

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
		Projectile p = projectilePool.GetPooledObject ().GetComponent<Projectile> ();
		UnityEngine.Assertions.Assert.IsNotNull (p);
		p.Init (shootPoint.position, dir, projectileSprite, "Player", projectileSpeed, 1);
		SoundManager.instance.RandomizeSFX (shootSound);
	}
}
