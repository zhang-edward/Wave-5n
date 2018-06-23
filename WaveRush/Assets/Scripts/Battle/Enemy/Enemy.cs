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
	public Vector2 srSize;
	public bool overrideSrSize;		// true = use the inspector value srSize for sprite size, false = use the sr bounds for sprite size

	public EntityPhysics body;
	public Animator anim;

	public bool hitDisabled{ get; private set; }

	[Header("Enemy Properties")]
	public int level;
	public int baseHealth = 1;
	public int maxHealth { get; set; }
	public int health { get; protected set; }
	public int moneyValueMultiplier = 1;
	public bool healable = true;
	public Vector3 healthBarOffset;
	public bool canBeDisabledOnHit = true;
	public bool invincible = false;

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

	private Player player;

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
		// init sr size values
		if (!overrideSrSize)
			srSize = sr.bounds.size;
		// init abilities
		InitAbilities ();
		if (OnEnemyInit != null)
			OnEnemyInit ();
		// init default values
		DEFAULT_LAYER = body.gameObject.layer;
		DEFAULT_SPEED = body.moveSpeed;

		this.level = level;
		this.map = map;
		maxHealth = EnemyHealthEquation(level, baseHealth);  // calculate health based on level
		health = maxHealth;
		deathPropPool = ObjectPooler.GetObjectPooler ("DeathProp");			// instantiate set object pooler

		// init movement and action
		movementMethod.Init(this, playerTransform);		
		action.Init (this, ToMoveState);

		player = playerTransform.GetComponentInChildren<Player>();

		Spawn (spawnLocation);
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireCube(transform.position + healthBarOffset, new Vector3(1f, 0.3f));
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


	// ============================== Spawn Methods ==============================
	private void Spawn(Vector3 spawnLocation)
	{
		switch (spawnMethod)
		{
		case SpawnMethod.WalkIn:
			StartCoroutine (WalkInSpawn (spawnLocation));
			break;
		case SpawnMethod.AnimateIn:
			StartCoroutine (AnimateIn (spawnLocation));
			break;
		case SpawnMethod.None:
			body.transform.position = spawnLocation;
			StartCoroutine (DEFAULT_STATE);
			break;
		}
	}

	protected virtual IEnumerator WalkInSpawn(Vector3 target)
	{
		body.gameObject.layer = LayerMask.NameToLayer ("EnemySpawnLayer");
		if (anim.ContainsParam("Moving"))
			anim.SetBool ("Moving", true);
		while (Vector3.Distance(transform.position, target) > 0.1f)
		{
			body.Move ((target - transform.position).normalized);
			yield return null;
		}
		body.gameObject.layer = DEFAULT_LAYER;
		StartCoroutine (DEFAULT_STATE);
		//Debug.Log ("Done");
	}

	protected virtual IEnumerator AnimateIn(Vector3 target)
	{
		body.transform.position = target;
		//UnityEngine.Assertions.Assert.IsTrue(anim.HasState(0, Animator.StringToHash("Spawn")));
		if (anim.HasState(0, Animator.StringToHash("Spawn")))
			anim.CrossFade ("Spawn", 0f);

		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("Spawn"))
			yield return null;
		StartCoroutine (DEFAULT_STATE);
	}

	// ============================== HitState and MoveState ==============================

	public virtual void Disable(float time)
	{
		StopAllCoroutines ();
		if (action != null)		// Special enemies have no action (trapped heroes)
			action.Interrupt();
		StartCoroutine (HitDisableState (time, 0));
	}

	private IEnumerator HitDisableState(float time, float randomImpulse)
	{
		// Set properties
		hitDisabled = true;
		body.ragdolled = true;
		body.AddRandomImpulse (randomImpulse);

		yield return new WaitForSeconds (time);

		anim.CrossFade ("default", 0f);
		hitDisabled = false;
		body.ragdolled = false;
		ToMoveState ();
		yield return null;
	}

	private void ResetColor()
	{
		sr.color = Color.white;
	}

	protected virtual IEnumerator MoveState()
	{
		yield return new WaitForEndOfFrame ();
		//print("Updating whatever moveState is: " + movementMethod);
		for (;;)
		{
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
		if (anim.HasState(0, Animator.StringToHash("default")))
			anim.CrossFade ("default", 0f);
		StopAllCoroutines ();       // stops any duplicate MoveStates that may have been started concurrently
		//print("To move state");
		StartCoroutine ("MoveState");
	}

	// ============================== Death ==============================
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
		int rangeValue = (int)Mathf.Sqrt(maxHealth);
		int moneyValue = Random.Range (rangeValue / 2, rangeValue * 2) * moneyValueMultiplier;
		if (moneyValue <= 0)
			return;
		GameObject o = ObjectPooler.GetObjectPooler(POOL_MONEY).GetPooledObject();
		o.SetActive(true);
		o.transform.position = transform.position;
		MoneyPickup moneyPickup = o.GetComponent<MoneyPickup>();
		moneyPickup.value = moneyValue;
		moneyPickup.Init(playerTransform);
	}

	// ============================== IDamageable Methods ==============================
	public virtual void Damage(int amt)
	{
		Damage(amt, false);
	}

	public virtual void Damage(int amt, bool disable)
	{
		if (invincible)
			return;
		health -= amt;
		if (OnEnemyDamaged != null)
			OnEnemyDamaged(amt);
		if (health > 0)
		{
			if (canBeDisabledOnHit && !hitDisabled && disable)
			{
				action.TryInterrupt();
				// Stop all states
				StopAllCoroutines();
				StartCoroutine(HitDisableState(0.05f, 3f));
			}
			sr.color = Color.red;
			Invoke("ResetColor", 0.2f);
		}
		else
		{
			Die();
		}
	}

	public virtual void Die()
	{
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

	// ============================== Misc Methods ==============================
	public int GetLevelDiff() {
		return player.hero.level - level;
	}

	protected virtual void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("MapBorder"))
		{
			if (OnCollideWithMapBorder != null)
				OnCollideWithMapBorder ();
		}	
	}

	public void DecreaseHealth(int amt)
	{
		health -= amt;
	}

	protected void RemoveEnemyFromList()
	{
		if (OnEnemyObjectDisabled != null)
			OnEnemyObjectDisabled(this);
	}

	private static int EnemyHealthEquation(int level, int baseHealth)
	{
		return Mathf.RoundToInt(baseHealth * (0.8f * level + 10) * (Mathf.Sqrt(level) / 2));
	}
}
