using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IDamageable
{
	public delegate void EnemyDamaged (float strength);
	public event EnemyDamaged OnEnemyDamaged;

	public delegate void EnemySelected (Enemy e);
	public event EnemySelected OnEnemyLastHit;

	public delegate void PlayerHealthChanged (int amt);
	public event PlayerHealthChanged OnPlayerDamaged;
	public event PlayerHealthChanged OnPlayerHealed;

	public delegate void PlayerLifecycleEvent();
	public event PlayerLifecycleEvent OnPlayerInitialized;
	public event PlayerLifecycleEvent OnPlayerDied;
	public event PlayerLifecycleEvent OnPlayerWillDie;

	[HideInInspector]
	public float DEFAULT_SPEED;

	public PlayerInfoHolder infoHolder;

	[Header("Entity Base Values")]
	public SpriteRenderer sr;
	public PlayerInput input;
	public EntityPhysics body;
	public Animator anim;

	[Header("Player Ability")]
	public PlayerHero hero;

	[Header("Universal Power Ups")]
	public HeroPowerUpHolder.HeroPowerUpDictionaryEntry[] powerUpPrefabs;

	[Header("Player direction")]
	public Vector2 dir;		// player's facing direction and movement direction

	[Header("Stats")]
	public int maxHealth = 10;
	public int health { get; private set; }
	private bool hitDisabled = false;			// true when the player has been damaged
	public bool isInvincible = false;			// property that can be set by other abilities

	public float damagedCooldownTime = 1.0f;

	[Header("AutoTargeter Object")]
	public Transform autoTargetReticle;
	[HideInInspector]
	public bool autoTargetEnabled = false;

	[Header("Effects")]
	public ParticleSystem healEffect;

	[Header("Audio")]
	public AudioClip hurtSound;
	public AudioClip deathSound;

	[HideInInspector]
	public ObjectPooler deathPropPool;
	[HideInInspector]
	public ObjectPooler effectPool;
/*	[HideInInspector]
	public Transform targetedEnemy;*/

	void Start()
	{
		deathPropPool = ObjectPooler.GetObjectPooler ("DeathProp");
		effectPool = ObjectPooler.GetObjectPooler ("Effect");
		DEFAULT_SPEED = body.moveSpeed;
		input.isInputEnabled = false;
	}

	public void Init(HeroType type)
	{
		InitPlayerHero (type);
		anim.runtimeAnimatorController = hero.animatorController;
		health = maxHealth;

		if (OnPlayerInitialized != null)
			OnPlayerInitialized ();
		StartCoroutine (SpawnState ());
	}

	private void InitPlayerHero(HeroType type)
	{
		this.hero = infoHolder.InitHero (type.ToString());
		hero.Init (body, anim, this);
	}

	/// <summary>
	/// Runs this coroutine at the start of the game
	/// </summary>
	/// <returns>The state.</returns>
	private IEnumerator SpawnState()
	{
		// Make sure the player animator has a state named "Spawn"
		UnityEngine.Assertions.Assert.IsTrue(anim.HasState(0, Animator.StringToHash("Spawn")));
		anim.CrossFade ("Spawn", 0f);

		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("Spawn"))
			yield return null;
		
		input.isInputEnabled = true;
	}

	/// <summary>
	/// Triggers the on enemy damaged event.
	/// </summary>
	/// <param name="damage">Damage.</param>
	public void TriggerOnEnemyDamagedEvent(int damage)
	{
		if (OnEnemyDamaged == null)
			return;
		OnEnemyDamaged (damage);
	}

	public void TriggerOnEnemyLastHitEvent(Enemy e)
	{
		if (OnEnemyLastHit == null)
			return;
		OnEnemyLastHit (e);
	}

	/// <summary>
	/// Flashes red. Used when the player is damaged by an enemy.
	/// </summary>
	public IEnumerator FlashRed()
	{
		hitDisabled = true;
		sr.color = Color.red;
		float t = 0;
		while (sr.color != Color.white)
		{
			sr.color = Color.Lerp (Color.red, Color.white, t);
			t += Time.deltaTime * 2f;
			yield return null;
		}
		hitDisabled = false;
	}


	/// <summary>
	/// Damage by the specified amt.
	/// </summary>
	/// <param name="amt">Amount to deduct from health.</param>
	public void Damage(int amt)
	{
		if (hitDisabled || isInvincible || amt <= 0)
			return;
		health -= amt;
		OnPlayerDamaged(amt);

		body.AddRandomImpulse (3f);
		StartCoroutine (FlashRed ());

		if (health <= 0 && OnPlayerWillDie != null)	
			OnPlayerWillDie ();	// for events that prevent the player's death
		// Player Died
		if (health <= 0)
		{
			UnityEngine.Assertions.Assert.IsNotNull (OnPlayerDied);
			OnPlayerDied ();
			SpawnDeathProps ();
			transform.parent.gameObject.SetActive (false);
			SoundManager.instance.PlayImportantSound (deathSound);
		}
		else
			SoundManager.instance.RandomizeSFX (hurtSound);
	}

	/// <summary>
	/// Heal the specified amt.
	/// </summary>
	/// <param name="amt">Amount to add to health.</param>
	public void Heal(int amt)
	{
		health += amt;
		if (health >= maxHealth)
			health = maxHealth;
		OnPlayerHealed (amt);
	}

	public void HealEffect (int amt, bool playEffect)
	{
		if (playEffect)
			healEffect.Play ();
		Heal (amt);
	}

	void Update()
	{
/*		if (autoTargetEnabled)
		{
			AutoTarget ();
			if (targetedEnemy != null)
				autoTargetReticle.position = targetedEnemy.position;
		}*/
	}

/*	public void AutoTarget()
	{
		RaycastHit2D[] raycastHits = Physics2D.CircleCastAll (transform.position, 1f, dir, 8f);
		Debug.DrawRay (transform.position, dir.normalized * 8f, Color.white);
		foreach (RaycastHit2D raycastHit in raycastHits)
		{
			Collider2D col = raycastHit.collider;
			if (col.CompareTag("Enemy"))
			{
				targetedEnemy = col.transform;
			}
		}
	}

	public void StartAutoTarget()
	{
		autoTargetEnabled = true;
		autoTargetReticle.gameObject.SetActive (true);
	}

	public void StopAutoTarget()
	{
		autoTargetEnabled = false;
		targetedEnemy = null;
		autoTargetReticle.position = new Vector3 (-1, -1, -1);
		autoTargetReticle.gameObject.SetActive (false);
	}*/

	private void SpawnDeathProps()
	{
		foreach (Sprite sprite in hero.deathProps)
		{
			//GameObject o = Instantiate (deathPropPrefab, transform.position, Quaternion.identity) as GameObject;
			//o.transform.SetParent (this.transform);
			GameObject o = deathPropPool.GetPooledObject();
			o.GetComponent<TempObject> ().Init (
				Quaternion.Euler(new Vector3(0, 0, 360f)),
				this.transform.position,
				sprite);
			o.GetComponent<Rigidbody2D> ().AddTorque (Random.Range (-50f, 50f));
			o.GetComponent<Rigidbody2D> ().AddForce (new Vector2(
				Random.value - 0.5f,
				Random.value - 0.5f),
				ForceMode2D.Impulse);
		}
	}

	public void SetPause(bool paused)
	{
		if (paused)
			Time.timeScale = 0;
		else
			Time.timeScale = 1;
	}

	public void StartTempSlowDown(float time)
	{
		StartCoroutine (TempSlowDown(time));
	}

	private IEnumerator TempSlowDown(float time)
	{
		Time.timeScale = 0.2f;
		yield return new WaitForSeconds (time);
		Time.timeScale = 1f;
	}
}

