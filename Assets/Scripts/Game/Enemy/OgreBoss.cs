using UnityEngine;
using System.Collections;

public class OgreBoss : BossEnemy {
	
	private const int NO_ACTION = 0;
	private const int ATTACK = 1;
	private const int STOMP = 2;

	private float attackTimer;
	private float roarTimer;

	[Header("OgreBoss Properties")]
	public Vector3 clubHitboxOffset;
	public Vector3 stompHitboxOffset;
	private Vector3 hitboxOffset;

	public float actionCooldownTime = 5.0f;
	[Header("Minions")]
	public GameObject[] minions;
	[Header("Attack Properties")]
	public float chargeTime;
	public float attackTime;
	[Header("Stomp Properties")]
	public float stompChargeTime;
	public float stompTime;
	[Header("Roar Properties")]
	public float roarChargeTime;
	public float roarTime;

	[Header("Ogre On Hit effects")]
	public SimpleAnimation rocksAnim;
	public SimpleAnimation dustAnim;
	private ObjectPooler effectPool;

	[Header("Audio")]
	public AudioClip[] clubSmashSounds;
	public AudioClip roarSound;
	public AudioClip spawnSound;

	void Start()
	{
		effectPool = ObjectPooler.GetObjectPooler ("Effect");
	}

	// Update is called once per frame
	void Update () {
		if (attackTimer > 0)
			attackTimer -= Time.deltaTime;
		if (roarTimer > 0)
			roarTimer -= Time.deltaTime;
		
		if (sr.flipX)
			hitboxOffset = new Vector3 (-clubHitboxOffset.x, clubHitboxOffset.y);
		else
			hitboxOffset = new Vector3 (clubHitboxOffset.x, clubHitboxOffset.y);
	}
		
	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position + stompHitboxOffset, 1f);
	}

	protected override IEnumerator MoveState()
	{
		UnityEngine.Assertions.Assert.IsTrue (body.moveSpeed == DEFAULT_SPEED);
		moveState = new WalkVicinityState (this);
		while (true)
		{
			moveState.UpdateState ();
			if (attackTimer <= 0)
			{
				if (roarTimer <= 0)
				{
					StartCoroutine ("RoarState");
					yield break;
				}
				switch (PlayerInRangeModes()) 
				{
				case ATTACK:
					StartCoroutine ("AttackState");
					yield break;
				case STOMP:
					StartCoroutine ("StompState");
					yield break;
				}
			}
			yield return null;
		}
	}

	protected override IEnumerator AnimateIn (Vector3 target)
	{
		SoundManager.instance.PlaySingle (spawnSound);
		return base.AnimateIn (target);
	}

	// returns an int id for which action to take when the player is in certain locations
	private int PlayerInRangeModes()
	{
		// check if ogre can use ATTACK
		Collider2D[] check = Physics2D.OverlapCircleAll (transform.position + hitboxOffset, playerDetectionRange);
		foreach (Collider2D col in check)
		{
			if (col.CompareTag ("Player"))
			{
				return ATTACK;
			}
		}
		// check if ogre can use STOMP
		check = Physics2D.OverlapCircleAll (transform.position, playerDetectionRange * 1.5f);
		foreach (Collider2D col in check)
		{
			if (col.CompareTag ("Player"))
			{
				return STOMP;
			}
		}
		return NO_ACTION;
	}

/*	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position + hitboxOffset, 1f);
	}*/

	private IEnumerator AttackState()
	{
		//UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
		//Debug.Log ("attacking: enter");
		attackTimer = actionCooldownTime;	
		body.Move (Vector2.zero);

		anim.SetTrigger ("Charge");
		yield return new WaitForSeconds (chargeTime);

		// Attack animation
		anim.SetTrigger ("Attack");
		yield return new WaitForSeconds (0.3f);	// wait for the animation to reach the attacking part
		OnClubHitGround();

		yield return new WaitForSeconds (attackTime);

		// Reset vars
		body.moveSpeed = DEFAULT_SPEED;

		anim.CrossFade ("Recharge", 0f);
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("Recharge"))
			yield return null;

		StartCoroutine ("MoveState");
	}

	private IEnumerator StompState()
	{
		//UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
		//Debug.Log ("attacking: enter");
		attackTimer = actionCooldownTime;	
		body.Move (Vector2.zero);

		anim.SetTrigger ("StompCharge");
		yield return new WaitForSeconds (stompChargeTime);
		anim.ResetTrigger ("StompCharge");	// prevents some glitchiness from happening

		// Attack animation
		anim.SetTrigger ("Stomp");
		OnStomp ();
		yield return new WaitForSeconds (stompTime);
	
		sr.color = Color.white;
		// Reset vars
		body.moveSpeed = DEFAULT_SPEED;
		anim.CrossFade ("Move", 0f);

		StartCoroutine ("MoveState");
	}

	private IEnumerator RoarState()
	{
		//UnityEngine.Assertions.Assert.IsTrue (state == State.Attacking);
		//Debug.Log ("attacking: enter");
		attackTimer = actionCooldownTime;
		roarTimer = 10.0f;

		anim.SetTrigger ("RoarCharge");
		yield return new WaitForSeconds (roarChargeTime);
		anim.ResetTrigger ("RoarCharge");	// prevents some glitchiness from happening

		// Attack animation
		anim.SetTrigger ("Roar");
		// wait for actual roaring part
		yield return new WaitForSeconds (0.2f);
		OnRoar ();
		yield return new WaitForSeconds (roarTime);

		// Reset vars
		body.moveSpeed = DEFAULT_SPEED;
		anim.CrossFade ("Move", 0f);

		StartCoroutine ("MoveState");
	}

	protected override void ResetVars()
	{
		body.gameObject.layer = DEFAULT_LAYER;
		body.moveSpeed = DEFAULT_SPEED;
	}

	// Club hit ground effect
	private void OnClubHitGround()
	{
		SoundManager.instance.RandomizeSFX (clubSmashSounds [Random.Range (0, clubSmashSounds.Length)]);
		Collider2D[] cols = Physics2D.OverlapCircleAll (hitboxOffset + transform.position, 1.5f);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Player"))
			{
				Player player = col.GetComponentInChildren<Player>();
				player.Damage (2);
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
			dustAnim.TimeLength);
		dustAnimPlayer.Play ();
	}

	// Stomp effect
	private void OnStomp()
	{
		SoundManager.instance.RandomizeSFX (clubSmashSounds [Random.Range (0, clubSmashSounds.Length)]);
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position + stompHitboxOffset, 1f);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Player"))
			{
				Player player = col.GetComponentInChildren<Player>();
				player.Damage (1);
			}
		}
		CameraControl.instance.StartShake (0.3f, 0.05f);
		StompEffect ();
	}

	private void StompEffect()
	{
		TempObject dust = effectPool.GetPooledObject ().GetComponent<TempObject>();
		SimpleAnimationPlayer dustAnimPlayer = dust.GetComponent<SimpleAnimationPlayer> ();
		dustAnimPlayer.anim = dustAnim;
		dust.Init (Quaternion.identity, 
			transform.position + stompHitboxOffset, 
			dustAnim.frames [0], 
			true, 
			0f, 
			0f, 
			dustAnim.TimeLength);
		dustAnimPlayer.Play ();
	}

	// Roar effect
	private void OnRoar()
	{
		SoundManager.instance.RandomizeSFX (roarSound);
		// Spawn 4 random enemies
		for(int i = 0; i < 4; i ++)
		{
			// place spawned enemies within a radius of 4 from this boss entity
			enemyManager.SpawnEnemy (minions [Random.Range (0, minions.Length)],
				UtilMethods.RandomOffsetVector2(transform.position, 4.0f));
		}
		CameraControl.instance.StartShake (1.5f, 0.03f);
	}
}
