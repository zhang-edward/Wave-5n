using UnityEngine;
using System.Collections;

public class ShootAction : EnemyAction {

	private Animator anim;
	private EntityPhysics body;
	private ObjectPooler projectilePool;
	private Vector3 shootPointPos;
	[Header("Set from Hierarchy")]
	public Sprite projectileSprite;
	public Transform shootPoint;
	[Header("Properties")]
	public float projectileSpeed = 5;
	public float chargeTime = 0.5f;
	public float attackTime = 0.2f;
	public int damage;
	public bool reflectX = true;
	[Header("AnimationStates")]
	public string chargeState;
	public string shootState;
	[Header("Audio")]
	public AudioClip shootSound;

	public event OnActionStateChanged onShoot;

	public override void Init (Enemy e, OnActionStateChanged onActionFinished)
	{
		base.Init (e, onActionFinished);
		anim = e.anim;
		body = e.body;
		projectilePool = ObjectPooler.GetObjectPooler ("Projectile");
		shootPointPos = shootPoint.localPosition;
	}

	public override void Execute ()
	{
		base.Execute ();
		StartCoroutine (UpdateState ());
	}

	public override void Interrupt ()
	{
		if (!interruptable)
			return;
		StopAllCoroutines ();
	}

	private IEnumerator UpdateState()
	{
		//UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
		//Debug.Log ("attacking: enter");
		// Charge up before attack
		Charge();
		yield return new WaitForSeconds (chargeTime);

		if (e.sr.flipX)
			shootPoint.localPosition = new Vector3(shootPointPos.x * -1, shootPointPos.y);
		else
			shootPoint.localPosition = new Vector3(shootPointPos.x, shootPointPos.y);

		Vector2 dir = e.player.transform.position - shootPoint.position;
		// Shoot
		Shoot(dir.normalized);

		if (onShoot != null)
			onShoot ();
		yield return new WaitForSeconds (attackTime);

		if (onActionFinished != null)
			onActionFinished ();
	}

	private void Charge()
	{
		anim.CrossFade (chargeState, 0f);		// triggers are unreliable, crossfade forces state to execute
		body.Move (Vector2.zero);
		//+ new Vector2(Random.value, Random.value);		// add a random offset
	}

	private void Shoot(Vector2 dir)
	{
		anim.CrossFade (shootState, 0f);		// triggers are unreliable, crossfade forces state to execute
		Projectile p = projectilePool.GetPooledObject ().GetComponent<Projectile> ();
		UnityEngine.Assertions.Assert.IsNotNull (p);
		p.Init (shootPoint.position, dir, projectileSprite, "Player", projectileSpeed, damage);
		SoundManager.instance.RandomizeSFX (shootSound);
	}
}
