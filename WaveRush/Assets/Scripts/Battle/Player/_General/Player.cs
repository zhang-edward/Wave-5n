using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour, IDamageable
{
	public const float	HIT_DISABLE_TIME = 0.5f;
	public const float	SOFT_HEALTH_RECOVERY_DELAY = 1.0f;
	public const float	SOFT_HEALTH_DECAY_RATE = 10f;
	public const int	BASE_HEALTH_PER_HEART = 40;

	[HideInInspector]public float DEFAULT_SPEED;

	public GameObject[] heroPrefabs;

	[Header("Entity Base Values")]
	public SpriteRenderer 	  sr;
	public AnimationSetPlayer animPlayer;
	public EntityPhysics      body;
	public CircleCollider2D   hurtbox;
	private PlayerInput		  input;

	[Header("Player Ability")]
	public PlayerHero hero;
	private Pawn[] party;
	private GameObject[] heroList;

	[Header("Player direction")]
	public Vector2   dir;		// player's facing direction and movement direction
	public Transform dirIndicator;

	[Header("Stats")]
	public int  maxHealth;
	public int  hardHealth { get; private set; }
	public int	softHealth { get; private set; }
	public StatusTimers invincibility = new StatusTimers();
	public StatusTimers inputDisabled = new StatusTimers();
	private int softHealthTarget;

	[Header("Effects")]
	public ParticleSystem  healEffect;
	public SimpleAnimation spawnEffect;
	public SimpleAnimation deathEffect;
	public SimpleAnimation sacrificeEffect;
	public SimpleAnimation parryEffect;
	public SimpleAnimation heroSwitchEffect;

	private SoundManager sound;
	[Header("Audio")]
	public AudioClip hurtSound;
	public AudioClip gameOverSound;
	public AudioClip heartBreakBuildUpSound;
	public AudioClip heartBreakSound;
	public AudioClip heartExtractSound;
	public AudioClip parrySound;
	public AudioClip parrySuccessSound;

	[HideInInspector] public ObjectPooler deathPropPool;
	[HideInInspector] public int activePartyMember;

	/** Delegates and Events */
	public delegate void EnemyDamaged(float strength);
	public event EnemyDamaged OnEnemyDamaged;

	public delegate void EnemySelected(Enemy e);
	public event EnemySelected OnEnemyLastHit;

	public delegate void PlayerHealthUpdate(int amt);
	public event PlayerHealthUpdate OnPlayerDamaged;
	public event PlayerHealthUpdate OnPlayerHealed;

	public delegate void PlayerLifecycleEvent();
	public event PlayerLifecycleEvent OnPlayerInitialized;
	public event PlayerLifecycleEvent OnPlayerDied;
	public event PlayerLifecycleEvent OnPlayerWillDie;

	public delegate void PlayerTargetedEnemyEvent(IDamageable enemy);
	public event PlayerTargetedEnemyEvent OnPlayerTryHit;


	public delegate void PlayerUpgradesUpdated(int numUpgrades);
	public event PlayerUpgradesUpdated OnPlayerUpgradesUpdated;

	private Coroutine updateSoftHealthRoutine;

#region Initialization
	void Awake() {
		input = GetComponent<PlayerInput>();
	}

	void Start() {
		deathPropPool = ObjectPooler.GetObjectPooler("DeathProp");
		DEFAULT_SPEED = body.moveSpeed;
		input.isInputEnabled = false;
		inputDisabled.OnTimerOn += () => { input.isInputEnabled = false; };
		inputDisabled.OnTimerOff += () => { input.isInputEnabled = true; };
		sound = SoundManager.instance;
	}

	public void SetParty(params Pawn[] party) {
		this.party = party;
		heroList = new GameObject[party.Length];
		for (int i = 0; i < party.Length; i ++) {
			InitPlayerHero(party[i], i);
			heroList[i].gameObject.SetActive(false);
		}
	}

	public void SetHero(int index) {
		EffectPooler.PlayEffect(heroSwitchEffect, transform.position);
		activePartyMember = index;
		for (int i = 0; i < party.Length; i ++) {
			// Enable active hero
			if (i == index) {
				heroList[i].gameObject.SetActive(true);
				hero = heroList[i].GetComponent<PlayerHero>();
				maxHealth = hero.numHearts * hero.healthPerHeart;
				hardHealth = softHealth = softHealthTarget = hero.hardHealth;
			}
			// Disable all inactive heroes
			else
				heroList[i].gameObject.SetActive(false);
		}
		// Initialize input listeners
		input.Init();
		// Send event
		if (OnPlayerInitialized != null)
			OnPlayerInitialized();
		// Play spawn state
		StartCoroutine(SpawnState());
	}

	private void InitPlayerHero(Pawn heroData, int index)
	{
		// Find the correct hero prefab and instantiate it
		foreach (GameObject prefab in heroPrefabs)
		{
			PlayerHero prefabHero = prefab.GetComponent<PlayerHero>();
			if (prefabHero.heroType == heroData.type)
			{
				GameObject o = Instantiate(prefab, transform.position, Quaternion.identity) as GameObject;
				heroList[index] = o;
				o.transform.SetParent(this.transform);
				hero = o.GetComponent<PlayerHero>();
				print ("Initializing player hero...");
				hero.Init(body, this, heroData);
				return;
			}
		}
		throw new UnityEngine.Assertions.AssertionException(this.GetType() + ".cs", "Cannot find hero with name " + heroData.type.ToString() + "!");
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
#endregion
#region Update and callbacks
	/** Update Timers */
	void Update()
	{
		hero.hardHealth = hardHealth;
		invincibility.DecrementTimer(Time.deltaTime);
		inputDisabled.DecrementTimer(Time.deltaTime);
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
#endregion
#region IDamageable and damage/death handling
	public void Damage(int amt, IDamageable source) {
		// Pre-damage effects
		if (OnPlayerTryHit != null)
			OnPlayerTryHit(source);
		if (invincibility.IsOn() || amt <= 0)
			return;

		// Damage
		hardHealth -= amt;
		softHealth = softHealthTarget;
		softHealthTarget = hardHealth;
		UpdateSoftHealth();
		body.AddRandomImpulse (3f);
		HitDisable(HIT_DISABLE_TIME);
		FlashColor(Color.red);

		// Post-damage effects
		if (OnPlayerDamaged != null)
			OnPlayerDamaged(amt);

		if (hardHealth <= 0 && OnPlayerWillDie != null)	
			OnPlayerWillDie ();	// For events that prevent the player's death
		// Player Died
		if (hardHealth <= 0) {	// Check if the player still has health left
			StartCoroutine(DieRoutine());
		}
		else
			sound.RandomizeSFX (hurtSound);
	}

	public void HitDisable(float time) {
		inputDisabled.Add(time / 2f);
		invincibility.Add(time);
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


	/// <summary>
	/// Heal the specified amt.
	/// </summary>
	/// <param name="amt">Amount to add to health.</param>
	public void Heal(int amt)
	{
		hardHealth += amt;
		if (hardHealth >= maxHealth)
			hardHealth = maxHealth;
		softHealth = softHealthTarget = hardHealth;
		if (OnPlayerHealed != null)
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

	private void UpdateSoftHealth() {
		if (updateSoftHealthRoutine != null)
			StopCoroutine(updateSoftHealthRoutine);
		updateSoftHealthRoutine = StartCoroutine(UpdateSoftHealthRoutine());
	}

	private IEnumerator UpdateSoftHealthRoutine() {
		yield return new WaitForSeconds(SOFT_HEALTH_RECOVERY_DELAY);
		while (softHealth > softHealthTarget) {
			softHealth --;
			yield return new WaitForSeconds(1f / SOFT_HEALTH_DECAY_RATE);
		}
	}

	// private IEnumerator SacrificeRoutine()
	// {
	// 	sound.RandomizeSFX(gameOverSound);

	// 	CameraControl.instance.SetOverlayColor(Color.black, 0.4f, 1.0f);
	// 	sr.sortingLayerName = "UI";
	// 	Time.timeScale = 0f;
	// 	yield return new WaitForSecondsRealtime(0.2f);
	// 	PlayDeathEffect(transform.position, sacrificeEffect);
	// 	sound.PlaySingle(heartBreakBuildUpSound);
	// 	sr.enabled = false;
	// 	yield return new WaitForSecondsRealtime(sacrificeEffect.GetSecondsUntilFrame(4));
	// 	CameraControl.instance.StartFlashColor(Color.white, 0.4f, 0, 0, 1f);
	// 	sound.RandomizeSFX(heartExtractSound);
	// 	yield return new WaitForSecondsRealtime(1.0f);
	// 	transform.parent.gameObject.SetActive(false);
	// 	Time.timeScale = 1f;
	// 	CameraControl.instance.DisableOverlay(1.0f);
	// }

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
#endregion
#region Effects and timescaling
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

	/// <summary>
	/// The player sprite flashes a color. Used when the player is damaged by an enemy.
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
#endregion
}