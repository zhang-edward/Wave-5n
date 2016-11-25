using UnityEngine;
using System.Collections;

public class OgreBoss : Enemy {

	private float attackTimer;

	[Header("OgreBoss Properties")]
	public Vector3 clubHitboxOffset;
	private Vector3 hitboxOffset;

	public float chargeTime = 0.5f;
	public float attackTime = 0.2f;
	public float cooldownTime = 1.0f;

	[Header("Ogre On Hit effects")]
	public SimpleAnimation rocksAnim;
	public SimpleAnimation dustAnim;
	private ObjectPooler effectPool;

	[Header("Audio")]
	public AudioClip[] clubSmashSounds;
	public AudioClip spawnSound;

	void Start()
	{
		effectPool = ObjectPooler.GetObjectPooler ("Effect");
	}

	// Update is called once per frame
	void Update () {
		if (attackTimer > 0)
			attackTimer -= Time.deltaTime;
		
		if (sr.flipX)
			hitboxOffset = new Vector3 (-clubHitboxOffset.x, clubHitboxOffset.y);
		else
			hitboxOffset = new Vector3 (clubHitboxOffset.x, clubHitboxOffset.y);
	}
		
	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position + clubHitboxOffset, 2f);
	}

	protected override IEnumerator MoveState()
	{
		UnityEngine.Assertions.Assert.IsTrue (body.moveSpeed == DEFAULT_SPEED);
		moveState = new WalkVicinityState (this);
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

	protected override IEnumerator AnimateIn (Vector3 target)
	{
		SoundManager.instance.PlaySingle (spawnSound);
		return base.AnimateIn (target);
	}

	protected override bool PlayerInRange()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position + hitboxOffset, playerDetectionRange);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag ("Player"))
			{
				return true;
			}
		}
		return false;
	}

/*	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position + hitboxOffset, 1f);
	}*/

	private IEnumerator AttackState()
	{
		//UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
		//Debug.Log ("attacking: enter");
		attackTimer = cooldownTime;	

		anim.SetTrigger ("Charge");
		body.moveSpeed = 0;
		body.Move (Vector2.zero);
		yield return new WaitForSeconds (chargeTime);

		// Attack animation
		anim.SetTrigger ("Attack");
		body.Move (Vector2.zero);
		yield return new WaitForSeconds (0.3f);	// wait for the animation to reach the attacking part
		OnClubHitGround();
		body.Move (Vector2.zero);


		yield return new WaitForSeconds (attackTime);

		// Reset vars
		body.moveSpeed = DEFAULT_SPEED;

		anim.CrossFade ("Recharge", 0f);
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("Recharge"))
			yield return null;

		StartCoroutine ("MoveState");
	}

	protected override void ResetVars()
	{
		body.gameObject.layer = DEFAULT_LAYER;
		body.moveSpeed = DEFAULT_SPEED;
	}

	private void OnClubHitGround()
	{
		SoundManager.instance.RandomizeSFX (clubSmashSounds [Random.Range (0, clubSmashSounds.Length)]);
		Collider2D[] cols = Physics2D.OverlapCircleAll (hitboxOffset + transform.position, 2f);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Player"))
			{
				Player player = col.GetComponentInChildren<Player>();
				player.Damage (3);
			}
		}
		CameraControl.instance.StartShake (0.3f, 0.05f);
		ClubHitEffect ();
	}

	private void ClubHitEffect()
	{
		TempObject rocks = effectPool.GetPooledObject ().GetComponent<TempObject>();
		SimpleAnimationPlayer rocksAnimPlayer = rocks.GetComponent<SimpleAnimationPlayer> ();
		rocksAnimPlayer.anim = rocksAnim;
		rocks.Init (Quaternion.identity, 
			hitboxOffset + transform.position, 
			rocksAnim.frames [0], 
			true, 
			0f, 
			rocksAnim.TimeLength, 
			2f);
		rocksAnimPlayer.Play ();

		TempObject dust = effectPool.GetPooledObject ().GetComponent<TempObject>();
		SimpleAnimationPlayer dustAnimPlayer = dust.GetComponent<SimpleAnimationPlayer> ();
		dustAnimPlayer.anim = dustAnim;
		dust.Init (Quaternion.identity, 
			hitboxOffset + transform.position, 
			dustAnim.frames [0], 
			true, 
			0f, 
			0f, 
			0.5f);
		dustAnimPlayer.Play ();
	}
}
