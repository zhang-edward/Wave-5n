using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Enemy : MonoBehaviour, IDamageable {

	protected string DEFAULT_STATE = "MoveState";
	protected int DEFAULT_LAYER;
	protected float DEFAULT_SPEED;
	public static int MAX_ABILITIES = 4;

	public enum MoveMethod {
		Follow,
		Bounce,
		WalkVicinty
	}

	public enum SpawnMethod {
		WalkIn,
		AnimateIn,
		None
	}

	[Header("Entity Base Properties")]
	public SpriteRenderer sr;
	public Vector2 srSize;
	public bool overrideSrSize;		// true = use the inspector value srSize for sprite size, false = use the sr bounds for sprite size
	[HideInInspector]
	public Transform player;
	public EntityPhysics body;
	public Animator anim;
	[HideInInspector]
	public Map map;		// used only for walk-in spawners

	public bool hitDisabled{ get; private set; }

	[Header("Enemy Properties")]
	public float playerDetectionRange = 2f;
	public bool canBeDisabledOnHit = true;
	public bool invincible = false;

	[Header("Abilities")]
	public List<EnemyAbility> abilities;

	[Header("Status")]
	public List<EnemyStatus> statuses;

	[Header("Spawn Properties")]
	public SpawnMethod spawnMethod;		// whether this enemy walks onto the play area or not

	[Header("Death Props")]
	//public Sprite deathSprite;
	public Sprite[] deathProps;
	private ObjectPooler deathPropPool;

	[Header("Enemy Stats")]
	public int maxHealth;
	public int health { get; private set; }
	public Vector3 healthBarOffset;

	[Header("Move State")]
	public MoveMethod movementMethod;
	protected IMoveState moveState;

	[HideInInspector]
	public GameObject moneyPickupPrefab;

	public delegate void EnemyInit();
	public event EnemyInit OnEnemyInit;
	public delegate void EnemyDamaged(int amt);
	public event EnemyDamaged OnEnemyDamaged;
	public delegate void EnemyDied();
	public event EnemyDied OnEnemyDied;
	public delegate void CollideWithMapBorder();
	public event CollideWithMapBorder OnCollideWithMapBorder;


	public virtual void Init(Vector3 spawnLocation, Map map)
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
		// set map
		this.map = map;
		// set stat
		health = maxHealth;
		deathPropPool = ObjectPooler.GetObjectPooler ("DeathProp");

		Spawn (spawnLocation);
	}

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

	public void AddStatus(GameObject statusObj)
	{
		EnemyStatus statusType = statusObj.GetComponent<EnemyStatus> ();
		// check if this enemy already has this status
		if (HasStatus (statusType))
		{
			Destroy (statusObj);
			return;
		}
		statusObj.transform.position = this.transform.position;
		statusObj.transform.SetParent (this.transform);

		EnemyStatus status = statusObj.GetComponent<EnemyStatus>();

		statuses.Add (status);
		status.Init (this);
	}

	private bool HasStatus(EnemyStatus status)
	{
		for (int i = statuses.Count - 1; i >= 0; i --)
		{
			EnemyStatus existingStatus = statuses [i];
			if (existingStatus == null)
				statuses.RemoveAt (i);
			else if (existingStatus.statusName.Equals (status.statusName))
				return true;
		}
		return false;
	}

	protected abstract void ResetVars();
	protected abstract IEnumerator MoveState();

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

	protected virtual bool PlayerInRange()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, playerDetectionRange);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag ("Player"))
			{
				return true;
			}
		}
		return false;
	}

	protected Player GetPlayerInRange()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, playerDetectionRange);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag ("Player"))
			{
				return col.GetComponentInChildren<Player>();
			}
		}
		return null;
	}

	// Hit Disable
	private IEnumerator HitDisableState(float time, float randomImpulse)
	{
		body.AddRandomImpulse (randomImpulse);
		hitDisabled = true;
		//Debug.Log ("Stopped all Coroutines");
		yield return new WaitForSeconds (time);
		hitDisabled = false;

		yield return new WaitForSeconds (0.2f);
		UnityEngine.Assertions.Assert.IsTrue(anim.HasState(0, Animator.StringToHash("default")));
		anim.CrossFade ("default", 0f);

		ResetVars ();

		// reset "flashRed" coroutine, in case it was interrupted
		invincible = false;
		sr.color = Color.white;

		StartCoroutine (DEFAULT_STATE);
		yield return null;
	}

	private IEnumerator FlashRed()
	{
		invincible = true;
		sr.color = Color.red;
		yield return new WaitForSeconds (0.2f);
		invincible = false;
		sr.color = Color.white;
	}

	protected void SpawnDeathProps()
	{
		foreach (Sprite sprite in deathProps)
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

	protected void SpawnMoneyPickup()
	{
		int value = Random.Range (0, maxHealth * 5);
		Instantiate (moneyPickupPrefab, transform.position, Quaternion.identity);
		moneyPickupPrefab.GetComponent<MoneyPickup> ().value = value;
	}

	// makes this enemy untouchable if outside map bounds (meaning it still needs to walk in bounds)
/*	public void CheckIfWithinMapBounds()
	{
		if (!map.WithinOpenCells (transform.position))
			invincible = true;
		else
			invincible = false;
	}*/

	public void Disable(float time)
	{
		StopAllCoroutines ();
		StartCoroutine (HitDisableState (time, 0));
	}

	// ===== IDamageable methods ===== //
	public virtual void Damage(int amt)
	{
		if (invincible)
			return;
		health -= amt;
		if (OnEnemyDamaged != null)
			OnEnemyDamaged (amt);
		if (health > 0)
		{
			if (canBeDisabledOnHit)
			{
				// Stop all states
				StopAllCoroutines ();
				StartCoroutine (HitDisableState (0.05f, 3f));
			}
			StartCoroutine (FlashRed ());
		}
		else
		{
			Die ();
		}
	}

	public virtual void Die()
	{
		ResetVars ();
		SpawnDeathProps ();
		SpawnMoneyPickup ();
		transform.parent.gameObject.SetActive (false);
		if (OnEnemyDied != null)
			OnEnemyDied ();
		Destroy (transform.parent.gameObject, 1.0f);
	}

	public virtual void Heal (int amt)
	{
		health += amt;
	}

	void OnDisable()
	{
		Destroy (gameObject, 1.0f);
	}

	protected virtual void OnTriggerEnter2D(Collider2D col)
	{
		if (col.CompareTag("MapBorder"))
		{
			if (OnCollideWithMapBorder != null)
				OnCollideWithMapBorder ();
		}	
	}

	/// <summary>
	/// Gets the IMoveState as specified by <see cref="movementMethod"/>
	/// </summary>
	/// <returns>The assigned move state.</returns>
	protected IMoveState GetAssignedMoveState()
	{
		switch (movementMethod)
		{
		case MoveMethod.Bounce:
			return new BounceState (this);
		case MoveMethod.Follow:
			return new FollowState (this);
		case MoveMethod.WalkVicinty:
			return new WalkVicinityState (this);
		default:
			return null;
		}
	}
}
