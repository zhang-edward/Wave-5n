using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IDamageable
{
	public const float HIT_DISABLE_TIME = 1.0f;

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
	public event PlayerLifecycleEvent OnPlayerTryHit;
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
	public Transform dirIndicator;

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
	public SimpleAnimation spawnEffect;
	public SimpleAnimation deathEffect;
	public SimpleAnimation sacrificeEffect;
	public SimpleAnimation parryEffect;

	[Header("Audio")]
	private SoundManager sound;
	public AudioClip hurtSound;
	public AudioClip gameOverSound;
	public AudioClip heartBreakBuildUpSound;
	public AudioClip heartBreakSound;
	public AudioClip heartExtractSound;
	public AudioClip parrySound;
	public AudioClip parrySuccessSound;
	public AudioClip parryFailSound;

	[HideInInspector]
	public ObjectPooler deathPropPool;

	void Start()
	{
		deathPropPool = ObjectPooler.GetObjectPooler ("DeathProp");
		DEFAULT_SPEED = body.moveSpeed;
		input.isInputEnabled = false;
		sound = SoundManager.instance;
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
	/// Flashes a color. Used when the player is damaged by an enemy.
	/// </summary>
	public void FlashColor(Color color, float time = 0.5f)
	{
		StartCoroutine(FlashColorRoutine(color, time));
	}

	private IEnumerator FlashColorRoutine(Color color, float time)
	{
		sr.color = color;
		float t = 0;
		while (sr.color != Color.white)
		{
			sr.color = Color.Lerp (color, Color.white, t);
			t += Time.deltaTime / time;
			yield return null;
		}
	}

	public void HitDisable(float time)
	{
		StartCoroutine(HitDisableRoutine(time));
	}

	private IEnumerator HitDisableRoutine(float time)
	{
		hitDisabled = true;
		yield return new WaitForSeconds(time);
		hitDisabled = false;
	}

	/// <summary>
	/// Makes the player sprite flash a color on and off again
	/// </summary>
	/// <param name="color">Color.</param>
	/// <param name="time">Time.</param>
	public void StrobeColor(Color color, float time)
	{
		StartCoroutine(StrobeColorRoutine(color, time));
	}

	private IEnumerator StrobeColorRoutine(Color color, float time)
	{
		float strobeSpeed = 0.03f;
		float t = 0;
		bool strobeColorOn = false;
		while (t < time)
		{
			if (!strobeColorOn)
				sr.color = color;
			else
				sr.color = Color.white;
			strobeColorOn = !strobeColorOn;
			t += strobeSpeed;
			yield return new WaitForSecondsRealtime(strobeSpeed);
		}
		sr.color = Color.white;
	}


	/// <summary>
	/// Damage by the specified amt.
	/// </summary>
	/// <param name="amt">Amount to deduct from health.</param>
	public void Damage(int amt)
	{
		if (OnPlayerTryHit != null)
			OnPlayerTryHit();
		if (hitDisabled || isInvincible || amt <= 0)
			return;
		health -= amt;
		OnPlayerDamaged(amt);

		body.AddRandomImpulse (3f);
		HitDisable(HIT_DISABLE_TIME);
		FlashColor(Color.red);

		if (health <= 0 && OnPlayerWillDie != null)	
			OnPlayerWillDie ();	// for events that prevent the player's death
		// Player Died
		if (health <= 0)
		{
			StartCoroutine(DieRoutine());
		}
		else
			sound.RandomizeSFX (hurtSound);
	}

	private IEnumerator DieRoutine()
	{
		sound.RandomizeSFX(gameOverSound);

		CameraControl.instance.SetOverlayColor(Color.black, 0.4f, 1.0f);
		sr.sortingLayerName = "UI";
		Time.timeScale = 0f;
		yield return new WaitForSecondsRealtime(0.5f);
		PlayDeathEffect(transform.position, deathEffect);
		sound.PlaySingle(heartBreakBuildUpSound);
		sr.enabled = false;
		yield return new WaitForSecondsRealtime(deathEffect.GetSecondsUntilFrame(10));
		CameraControl.instance.StartFlashColor(Color.white, 0.4f, 0, 0, 1f);
		sound.RandomizeSFX(heartBreakSound);
		yield return new WaitForSecondsRealtime(1.0f);
		transform.parent.gameObject.SetActive(false);
		Time.timeScale = 1f;
		CameraControl.instance.DisableOverlay(1.0f);
		if (OnPlayerDied != null)
			OnPlayerDied();
	}

	public void LeaveEffect()
	{
		//StartCoroutine(SacrificeRoutine());
	}

	private IEnumerator SacrificeRoutine()
	{
		sound.RandomizeSFX(gameOverSound);

		CameraControl.instance.SetOverlayColor(Color.black, 0.4f, 1.0f);
		sr.sortingLayerName = "UI";
		Time.timeScale = 0f;
		yield return new WaitForSecondsRealtime(0.2f);
		PlayDeathEffect(transform.position, sacrificeEffect);
		sound.PlaySingle(heartBreakBuildUpSound);
		sr.enabled = false;
		yield return new WaitForSecondsRealtime(sacrificeEffect.GetSecondsUntilFrame(4));
		CameraControl.instance.StartFlashColor(Color.white, 0.4f, 0, 0, 1f);
		sound.RandomizeSFX(heartExtractSound);
		yield return new WaitForSecondsRealtime(1.0f);
		transform.parent.gameObject.SetActive(false);
		Time.timeScale = 1f;
		CameraControl.instance.DisableOverlay(1.0f);
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

	/// <summary>
	/// Heal function with an option to play the healing effect
	/// </summary>
	/// <param name="amt">Amt.</param>
	/// <param name="playEffect">If set to <c>true</c> play effect.</param>
	public void HealEffect (int amt, bool playEffect)
	{
		if (playEffect)
			healEffect.Play ();
		Heal (amt);
	}

	/// <summary>
	/// Spawn death props
	/// </summary>
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

	/// <summary>
	/// Sets the timescale to 1 or 0
	/// </summary>
	/// <param name="paused">If set to <c>true</c> pause.</param>
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

	private void PlayDeathEffect(Vector3 position, SimpleAnimation effect)
	{
		GameObject o = EffectPooler.instance.GetPooledObject();
		SimpleAnimationPlayer animPlayer = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
		tempObj.info = new TempObjectInfo(true, 0f, effect.TimeLength - 2f, 1f);
		animPlayer.anim = effect;
		tempObj.Init(Quaternion.identity,
					 position,
					 effect.frames[0]);
		animPlayer.ignoreTimeScaling = true;
		animPlayer.Play();
	}
}