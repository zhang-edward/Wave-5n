using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour, IDamageable {

	protected string DEFAULT_STATE = "MoveState";
	protected int DEFAULT_LAYER;
	protected float DEFAULT_SPEED;

	public enum MoveMethod {
		Follow,
		Bounce,
		WalkVicinty
	}

	[Header("Entity Base Properties")]
	public SpriteRenderer sr;
	[HideInInspector]
	public Transform player;
	public EntityPhysics body;
	public Animator anim;
	[HideInInspector]
	public Map map;		// used only for walk-in spawners

	public bool hitDisabled{ get; private set; }
	public bool confused { get; private set; }

	[Header("Enemy Properties")]
	public float playerDetectionRange = 2f;
	public bool canBeDisabledOnHit = true;
	public bool invincible = false;

	[Header("Spawn Properties")]
	public bool walkIn = true;		// whether this enemy walks onto the play area or not

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

	public delegate void EnemyDied();
	public event EnemyDied OnEnemyDied;
	public delegate void CollideWithMapBorder();
	public event CollideWithMapBorder OnCollideWithMapBorder;


	public virtual void Init(Vector3 spawnLocation, Map map)
	{
		this.map = map;
		health = maxHealth;
		DEFAULT_LAYER = body.gameObject.layer;
		deathPropPool = ObjectPooler.GetObjectPooler ("DeathProp");
		DEFAULT_SPEED = body.moveSpeed;
		Spawn (spawnLocation);
	}

	protected abstract void ResetVars();
	protected abstract IEnumerator MoveState();

	private void Spawn(Vector3 spawnLocation)
	{
		if (walkIn)
			StartCoroutine (WalkInSpawn (spawnLocation));
		else
			StartCoroutine (AnimateIn (spawnLocation));
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
		UnityEngine.Assertions.Assert.IsTrue(anim.HasState(0, Animator.StringToHash("Spawn")));
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
		StartCoroutine (DEFAULT_STATE);
		yield return null;
	}

	private IEnumerator FlashRed()
	{
		sr.color = Color.red;
		invincible = true;
		yield return new WaitForSeconds (0.2f);
		sr.color = Color.white;
		invincible = false;
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
		Destroy (this, 1.0f);
	}

	public virtual void Heal (int amt)
	{
		health += amt;
	}

	void OnDisable()
	{
		if (OnEnemyDied != null)
			OnEnemyDied ();
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
