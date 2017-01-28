using UnityEngine;
using System.Collections;

public class ShootAction : EnemyAction {

	private Animator anim;
	private EntityPhysics body;
	private ObjectPooler projectilePool;
	[Header("Set from Hierarchy")]
	public Sprite projectileSprite;
	public Transform shootPoint;
	[Header("Properties")]
	public float projectileSpeed = 5;
	public float chargeTime = 0.5f;
	public float attackTime = 0.2f;
	public float cooldownTime = 1.0f;
	public int damage;
	[Header("Audio")]
	public AudioClip shootSound;

	public event OnActionStateChanged onShoot;

	public override void Init (Enemy e, OnActionStateChanged onActionFinished)
	{
		base.Init (e, onActionFinished);
		anim = e.anim;
		body = e.body;
		projectilePool = ObjectPooler.GetObjectPooler ("Projectile");
	}

	public override void Execute ()
	{
		StartCoroutine (UpdateState ());
	}

	public override void Interrupt ()
	{
		base.Interrupt ();
		StopAllCoroutines ();
	}

	private IEnumerator UpdateState()
	{
		//UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
		//Debug.Log ("attacking: enter");
		// Charge up before attack
		Vector3 dir;
		Charge(out dir);
		yield return new WaitForSeconds (chargeTime);

		// Shoot
		Shoot(dir.normalized);

		if (onShoot != null)
			onShoot ();
		yield return new WaitForSeconds (attackTime);

		onActionFinished ();
	}

	private void Charge(out Vector3 dir)
	{
		anim.CrossFade ("charge", 0f);		// triggers are unreliable, crossfade forces state to execute
		body.Move (Vector2.zero);
		dir = (Vector2)(e.player.position - transform.position); // freeze moving direction
		//+ new Vector2(Random.value, Random.value);		// add a random offset
	}

	private void Shoot(Vector2 dir)
	{
		anim.CrossFade ("attack", 0f);		// triggers are unreliable, crossfade forces state to execute
		Projectile p = projectilePool.GetPooledObject ().GetComponent<Projectile> ();
		UnityEngine.Assertions.Assert.IsNotNull (p);
		p.Init (shootPoint.position, dir, projectileSprite, "Player", projectileSpeed, damage);
		SoundManager.instance.RandomizeSFX (shootSound);
	}
}
