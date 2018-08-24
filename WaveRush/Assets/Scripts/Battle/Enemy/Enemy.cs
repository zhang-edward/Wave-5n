using UnityEngine;
using EnemyActions;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IDamageable {

	public const string POOL_MONEY = "Money";
	public static int SCALETYPE_DAMAGE = 0;
	public static int MAX_LEVEL_RAW = 50;
	public static int MAX_ABILITIES = 4;

	protected string DEFAULT_STATE = "MoveState";
	protected int DEFAULT_LAYER;

	[HideInInspector] public float DEFAULT_SPEED;
	[HideInInspector] public Transform playerTransform;
	[HideInInspector] public Map map;						// used only for walk-in spawners

	public enum SpawnMethod {
		WalkIn,
		AnimateIn,
		None
	}

	[Header("Entity Base Properties")]
	public SpriteRenderer sr;

	[Header("Enemy Offsets (err towards y = 0)")]
	public Vector3 healthBarPos;
	public Vector3 headPos;
	public Vector3 feetPos;
	public float statusIconSize;
	public bool overrideSrSize;		// true = use the inspector value for sprite size, false = use the sr bounds for sprite size


	public EntityPhysics body;
	// public Animator anim;
	public AnimationSetPlayer animationSetPlayer;

	public bool hitDisabled{ get; private set; }

	[Header("Enemy Properties")]
	public int level;
	public int baseHealth = 1;
	public int maxHealth { get; set; }
	public int health { get; protected set; }
	public int moneyValueMultiplier = 1;
	public bool healable = true;
	public bool canBeDisabled = true;
	public bool invincible = false;

	[Header("Animation")]
	public string movingAnimState = "Moving";
	public string spawnAnimState = "Spawn";
	public AnimationSet anim;

	[Header("Abilities and Status Effects")]
	public List<EnemyAbility> abilities;
	public List<EnemyStatus> statuses;

	[Header("Death Props")]
	//public Sprite deathSprite;
	public Sprite[] deathProps;
	protected ObjectPooler deathPropPool;

	[Header("Behavior")]
	public MoveState movementMethod;
	public EnemyAction action;
	public SpawnMethod spawnMethod;		// whether this enemy walks onto the arena or not

	protected Player player;
	private List<Color> colors = new List<Color>();		// The different colors (from status effects or other sources) that this instance will flash
	private Color baseColor = Color.white;	// The base color that this sprite is currently. Will reset to this when flashing any other color
	/** States */
	private Coroutine moveState;
	private Coroutine hitDisableState;
	private Coroutine spawnState;
	/** Color routine */
	private bool flashingColor;
	private Coroutine colorRoutine;

	public delegate void EnemyLifecycleEvent();
	public event EnemyLifecycleEvent OnEnemyInit;
	public event EnemyLifecycleEvent OnEnemyDied;
	public event EnemyLifecycleEvent OnCollideWithMapBorder;

	public delegate void EnemyDamaged(int amt);
	public event EnemyDamaged OnEnemyDamaged;

	public delegate void EnemyObjectDisabled(Enemy e);
	public event EnemyObjectDisabled OnEnemyObjectDisabled;


	public virtual void Init(Vector3 spawnLocation, Map map, int level)
	{
		// Sr size values
		if (!overrideSrSize)
			statusIconSize = sr.bounds.size.x;
		// Abilities
		InitAbilities ();
		if (OnEnemyInit != null)
			OnEnemyInit ();
		// Default values
		DEFAULT_LAYER = body.gameObject.layer;
		DEFAULT_SPEED = body.moveSpeed;

		this.level = level;
		this.map = map;
		maxHealth = EnemyHealthEquation(level, baseHealth);  // calculate health based on level
		health = maxHealth;
		deathPropPool = ObjectPooler.GetObjectPooler ("DeathProp");			// instantiate set object pooler

		// Animation
		anim.Init(animationSetPlayer);
		animationSetPlayer.Init();

		// Movement and action
		movementMethod.Init(this, playerTransform);		
		action.Init (this, ToMoveState);

		player = playerTransform.GetComponentInChildren<Player>();

		Spawn (spawnLocation);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(transform.position + healthBarPos, new Vector3(1f, 0.3f));
		Gizmos.DrawWireSphere(transform.position + headPos, 0.1f);
		Gizmos.DrawWireSphere(transform.position + feetPos, 0.1f);
	}

	// ============================== Abilities and Statuses ==============================
	private void InitAbilities()
	{
		foreach (EnemyAbility ability in abilities)
		{
			ability.Init (this);
		}
	}

	public void AddAbility(GameObject abilityPrefab)
	{
		GameObject o = Instantiate (abilityPrefab);
		o.transform.position = this.transform.position;
		o.transform.SetParent (this.transform);
		EnemyAbility enemyAbility = o.GetComponent<EnemyAbility> ();
		abilities.Add (enemyAbility);
	}

	public virtual void AddStatus(GameObject statusObj)
	{
		if (invincible)
			return;
		// check if this enemy already has this status
		EnemyStatus existingStatus = GetStatus (statusObj.GetComponent<EnemyStatus>().statusName);
		if (existingStatus != null)
		{
			existingStatus.Stack ();
			Destroy (statusObj);
			return;
		}
		statusObj.transform.position = this.transform.position;
		statusObj.transform.SetParent (this.transform);

		EnemyStatus status = statusObj.GetComponent<EnemyStatus>();

		statuses.Add (status);
		status.Init (this);
	}

	public EnemyStatus GetStatus(string statusName)
	{
		for (int i = statuses.Count - 1; i >= 0; i --)
		{
			EnemyStatus existingStatus = statuses [i];
			if (existingStatus == null)
				statuses.RemoveAt (i);
			else if (existingStatus.statusName.Equals (statusName))
				return existingStatus;
		}
		return null;
	}

#region Spawn Methods
	private void Spawn(Vector3 spawnLocation)
	{
		switch (spawnMethod)
		{
		case SpawnMethod.WalkIn:
			spawnState = StartCoroutine (WalkInSpawn (spawnLocation));
			break;
		case SpawnMethod.AnimateIn:
			spawnState = StartCoroutine (AnimateIn (spawnLocation));
			break;
		case SpawnMethod.None:
			body.transform.position = spawnLocation;
			StartCoroutine (MoveState());
			break;
		}
	}

	protected virtual IEnumerator WalkInSpawn(Vector3 target)
	{
		body.gameObject.layer = LayerMask.NameToLayer ("EnemySpawnLayer");
		anim.Play(movingAnimState);
		while (Vector3.Distance(transform.position, target) > 0.1f)
		{
			body.Move ((target - transform.position).normalized);
			yield return null;
		}
		body.gameObject.layer = DEFAULT_LAYER;
		StartCoroutine (MoveState());
		//Debug.Log ("Done");
	}

	protected virtual IEnumerator AnimateIn(Vector3 target)
	{
		body.transform.position = target;
		//UnityEngine.Assertions.Assert.IsTrue(anim.HasState(0, Animator.StringToHash("Spawn")));
		anim.Play(spawnAnimState);
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.player.isPlaying)
			yield return null;
		StartCoroutine (MoveState());
	}
#endregion
#region HitState and MoveState

	private void ForceStopAllStates() {
		// print ("All states stopped");
		if (moveState != null)
			StopCoroutine(moveState);
		if (hitDisableState != null)
			StopCoroutine(hitDisableState);
		if (spawnState != null)
			StopCoroutine(spawnState);
	}

	// This is what is generally used for attacks
	public virtual bool Disable(float time) {
		if (!canBeDisabled || !action.TryInterrupt())
			return false;
		hitDisableState = StartCoroutine (HitDisableState (time, 0));
		return true;
	}

	// Used to force this enemy to be disabled
	public virtual void ForceDisable(float time)
	{
		ForceStopAllStates();
		if (action != null)		// Special enemies have no action (trapped heroes)
			action.Interrupt();
		// print ("disabled");
		hitDisableState = StartCoroutine (HitDisableState (time, 0));
	}

	private IEnumerator HitDisableState(float time, float randomImpulse)
	{
		// Set properties
		hitDisabled = true;
		body.ragdolled = true;
		body.AddRandomImpulse (randomImpulse);

		yield return new WaitForSeconds (time);

		anim.player.ResetToDefault();
		hitDisabled = false;
		body.ragdolled = false;
		ToMoveState ();
		yield return null;
	}

	protected virtual IEnumerator MoveState()
	{
		yield return new WaitForEndOfFrame ();
		//print("Updating whatever moveState is: " + movementMethod);
		for (;;)
		{
			// print ("Doing movestate");
			movementMethod.UpdateState ();
			if (action.CanExecute ())
			{
				action.Execute ();
				yield break;
			}
			yield return null;
		}
	}
	
	private void ToMoveState()
	{
		// print ("To move state");
		anim.player.ResetToDefault();
		ForceStopAllStates();
		body.ragdolled = false;
		moveState = StartCoroutine (MoveState());
	}
#endregion
#region Death 
	protected virtual void SpawnDeathProps()
	{
		foreach (Sprite sprite in deathProps)
		{
			GameObject o = deathPropPool.GetPooledObject();
			Rigidbody2D rb2d = o.GetComponent < Rigidbody2D> ();
			o.GetComponent<TempObject> ().Init (
				Quaternion.Euler(new Vector3(0, 0, 360f)),
				this.transform.position,
				sprite);
			rb2d.AddTorque (Random.Range (-50f, 50f));
			rb2d.AddForce (new Vector2(
				Random.value - 0.5f,
				Random.value - 0.5f),
				ForceMode2D.Impulse);
		}
	}

	protected void SpawnMoneyPickup()
	{
		float rangeValue = Mathf.Sqrt(maxHealth);
		float rangeNoise = rangeValue * 0.2f;
		int moneyValue = (int)(Random.Range (rangeValue - rangeNoise, rangeValue + rangeNoise) * moneyValueMultiplier * 0.5f);
		if (moneyValue <= 0)
			return;
		GameObject o = ObjectPooler.GetObjectPooler(POOL_MONEY).GetPooledObject();
		o.SetActive(true);
		o.transform.position = transform.position;
		MoneyPickup moneyPickup = o.GetComponent<MoneyPickup>();
		moneyPickup.value = moneyValue;
		moneyPickup.Init(playerTransform);
	}
#endregion
#region IDamageable Methods
	public virtual void Damage(int amt, IDamageable source)
	{
		Damage(amt, source, false);
	}

	public virtual void Damage(int amt, IDamageable source, bool disable)
	{
		if (invincible)
			return;
		health -= amt;
		if (OnEnemyDamaged != null)
			OnEnemyDamaged(amt);
		if (health > 0)
		{
			if (canBeDisabled && !hitDisabled && disable)	{
				action.TryInterrupt();
				// Stop all states
				StopAllCoroutines();
				StartCoroutine(HitDisableState(0.05f, 3f));
			}
			StartCoroutine(FlashColor(Color.red));
		}
		else {
			Die();
		}
	}

	private IEnumerator FlashColor(Color color) {
		sr.color = color;
		flashingColor = true;
		yield return new WaitForSeconds(0.2f);
		sr.color = baseColor;
		flashingColor = false;
	}

	public virtual void Die() {
		if (OnEnemyDied != null)
			OnEnemyDied ();
		//ResetVars ();
		SpawnDeathProps ();
		SpawnMoneyPickup ();
		transform.parent.gameObject.SetActive (false);
		if (OnEnemyObjectDisabled != null)
			OnEnemyObjectDisabled (this);
		Destroy (transform.parent.gameObject, 1.0f);
	}

	public virtual void Heal (int amt)
	{
		health += amt;
		if (health > maxHealth)
			health = maxHealth;
	}
#endregion
#region Misc Methods
	private IEnumerator RotateColors() {
		for (;;) {
			if (colors.Count == 0) {
				if (!flashingColor)
					sr.color = Color.white;
				baseColor = Color.white;
				yield break;
			}
			for (int i = 0; i < colors.Count; i ++) {
				if (!flashingColor)
					sr.color = colors[i];
				baseColor = colors[i];
				yield return new WaitForSeconds(1f);
			}
		}
	}

	private void RestartColorRoutine() {
		if (colorRoutine != null) {
			StopCoroutine(colorRoutine);
		}
		colorRoutine = StartCoroutine(RotateColors());
	}

	public void AddColor(Color color) {
		if (!colors.Contains(color)) {
			colors.Add(color);
			RestartColorRoutine();
		}
	}

	public void RemoveColor(Color color) {
		if (colors.Contains(color)) {
			colors.Remove(color);
			RestartColorRoutine();
		}
		else
			Debug.LogWarning("Enemy did not have color " + color + " in its color list! Perhaps it was removed more than once?");
	}

	public int GetLevelDiff() {
		return player.hero.level - level;
	}

	protected virtual void OnTriggerEnter2D(Collider2D col) {
		if (col.CompareTag("MapBorder"))
		{
			if (OnCollideWithMapBorder != null)
				OnCollideWithMapBorder ();
		}	
	}

	public void DecreaseHealth(int amt) {
		health -= amt;
	}

	protected void RemoveEnemyFromList() {
		if (OnEnemyObjectDisabled != null)
			OnEnemyObjectDisabled(this);
	}

	private static int EnemyHealthEquation(int level, int baseHealth) {
		return Mathf.RoundToInt(baseHealth * (0.8f * level + 10) * (Mathf.Sqrt(level) / 2));
	}
#endregion
}
