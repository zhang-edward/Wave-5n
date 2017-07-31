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

	public delegate void PlayerUpgradesUpdated(int numUpgrades);
	public event PlayerUpgradesUpdated OnPlayerUpgradesUpdated;

	[HideInInspector]
	public float DEFAULT_SPEED;

	public GameObject[] heroPrefabs;

	[Header("Entity Base Values")]
	public SpriteRenderer sr;
	public AnimationSetPlayer animPlayer;
	public PlayerInput input;
	public EntityPhysics body;

	[Header("Player Ability")]
	public PlayerHero hero;

	[Header("Player direction")]
	public Vector2 dir;		// player's facing direction and movement direction

	[Header("Stats")]
	public int maxHealth = 10;
	public int health { get; private set; }
	private bool hitDisabled = false;			// true when the player has been damaged
	public bool isInvincible = false;			// property that can be set by other abilities

	public float damagedCooldownTime = 1.0f;

	/*[Header("AutoTargeter Object")]
	public Transform autoTargetReticle;
	[HideInInspector]
	public Transform targetedEnemy;
	[HideInInspector]
	public bool autoTargetEnabled = false;*/

	[Header("Effects")]
	public ParticleSystem healEffect;
	public SimpleAnimation deathEffect;

	[Header("Audio")]
	public AudioClip hurtSound;
	public AudioClip deathSound;
	public AudioClip poofSound;

	[HideInInspector]
	public ObjectPooler deathPropPool;

	void Start()
	{
		deathPropPool = ObjectPooler.GetObjectPooler ("DeathProp");
		DEFAULT_SPEED = body.moveSpeed;
		input.isInputEnabled = false;
	}

	public void Init(Pawn heroData)
	{
		InitPlayerHero (heroData);
		health = maxHealth;

		if (OnPlayerInitialized != null)
			OnPlayerInitialized ();
		StartCoroutine (SpawnState ());
	}

	private void InitPlayerHero(Pawn heroData)
	{
		// this.hero = infoHolder.CreateHero (type.ToString());
		foreach (GameObject prefab in heroPrefabs)
		{
			PlayerHero prefabHero = prefab.GetComponent<PlayerHero>();
//			print(prefabHero.heroType == type);
			if (prefabHero.heroType == heroData.type)
			{
				GameObject o = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
				o.transform.SetParent(this.transform);
				hero = o.GetComponent<PlayerHero>();
				hero.Init(body, this, heroData);
				return;
			}
		}
		throw new UnityEngine.Assertions.AssertionException(this.GetType() + ".cs",
															"Cannot find hero with name " + heroData.type.ToString() + "!");
	}

	/// <summary>
	/// Runs this coroutine at the start of the game
	/// </summary>
	/// <returns>The state.</returns>
	private IEnumerator SpawnState()
	{
		//print("Spawned");
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
			//SpawnDeathProps ();
			StartCoroutine(Die());
		}
		else
			SoundManager.instance.RandomizeSFX (hurtSound);
	}

	private IEnumerator Die()
	{
		SoundManager.instance.RandomizeSFX(deathSound);

		CameraControl.instance.SetOverlayColor(Color.black, 0.4f, 1.0f);
		sr.sortingLayerName = "UI";
		Time.timeScale = 0f;
		yield return new WaitForSecondsRealtime(1.5f);
		PlayDeathEffect(transform.position);
		CameraControl.instance.StartFlashColor(Color.white, 0.4f, 0, 0, 1f);
		SoundManager.instance.RandomizeSFX(poofSound);
		sr.enabled = false;
		yield return new WaitForSecondsRealtime(2.0f);
		Time.timeScale = 1f;
		CameraControl.instance.DisableOverlay(1.0f);
		transform.parent.gameObject.SetActive(false);
		if (OnPlayerDied != null)
			OnPlayerDied();
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
		yield return new WaitForSecondsRealtime (time);
		Time.timeScale = 1f;
	}

	public void AddUpgrades(int numUpgrades)
	{
		if (OnPlayerUpgradesUpdated != null)
			OnPlayerUpgradesUpdated(numUpgrades);
	}

	private void PlayDeathEffect(Vector3 position)
	{
		GameObject o = EffectPooler.instance.GetPooledObject();
		SimpleAnimationPlayer animPlayer = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
		tempObj.info = new TempObjectInfo(true, 0f, deathEffect.TimeLength, 0);
		animPlayer.anim = deathEffect;
		tempObj.Init(Quaternion.identity,
					 position,
		             deathEffect.frames[0]);
		animPlayer.ignoreTimeScaling = true;
		animPlayer.Play();
	}
}

