using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour, IDamageable {

	protected string DEFAULT_STATE = "MoveState";
	protected int DEFAULT_LAYER;
	protected float DEFAULT_SPEED;

	[Header("Entity Base Properties")]
	public SpriteRenderer sr;
	public Transform player;
	public EntityPhysics body;
	public Animator anim;
	[HideInInspector]
	public Map map;		// used only for walk-in spawners

	public bool hitDisabled{ get; private set; }
	public bool confused { get; private set; }

	[Header("Enemy Properties")]
	public bool isBoss = false;
	public float playerDetectionRange = 2f;
	public bool canBeDisabledOnHit = true;
	public bool canBeConfused = true;
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

	public delegate void EnemyDied();
	public event EnemyDied OnEnemyDied;

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
			CheckIfWithinMapBounds ();
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
	private IEnumerator HitDisableState()
	{
		body.AddRandomImpulse ();
		hitDisabled = true;
		//Debug.Log ("Stopped all Coroutines");
		yield return new WaitForSeconds (0.05f);
		hitDisabled = false;

		yield return new WaitForSeconds (0.2f);
		UnityEngine.Assertions.Assert.IsTrue(anim.HasState(0, Animator.StringToHash("default")));
		anim.CrossFade ("default", 0f);

		ResetVars ();
		StartCoroutine (DEFAULT_STATE);
		yield return null;
	}

	// Confuse
	private IEnumerator ConfusedState(float time)
	{
		confused = true;
		body.moveSpeed = 0;

		TempObject tempObj = ObjectPooler.GetObjectPooler ("Effect").GetPooledObject ().GetComponent<TempObject> ();
		SimpleAnimationPlayer animPlayer = tempObj.GetComponent<SimpleAnimationPlayer> ();
		animPlayer.anim = StatusEffectContainer.instance.confusedEffect;
		animPlayer.looping = true;
		tempObj.Init (Quaternion.identity,
			transform.position + healthBarOffset,
			animPlayer.anim.frames [0],
			true,
			0f,
			animPlayer.anim.TimeLength,
			0f);
		animPlayer.Play ();

		yield return new WaitForSeconds (time);
		confused = false;

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

	private void SpawnDeathProps()
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

	// makes this enemy untouchable if outside map bounds (still needs to walk in bounds on spawn)
	public void CheckIfWithinMapBounds()
	{
		if (!map.WithinOpenCells (transform.position))
			invincible = true;
		else
			invincible = false;
	}

	public void Confuse(float time)
	{
		if (!canBeConfused)
			return;
		StopAllCoroutines ();
		StartCoroutine(ConfusedState (time));
	}

	// ===== IDamageable methods ===== //
	public virtual void Damage(int amt)
	{
		if (invincible)
			return;
		// Stop all states
		health -= amt;

		if (health > 0)
		{
			if (canBeDisabledOnHit)
			{
				StopAllCoroutines ();
				StartCoroutine (HitDisableState ());
			}
			StartCoroutine (FlashRed ());
		}
		else
		{
			ResetVars ();
			SpawnDeathProps ();
			transform.parent.gameObject.SetActive (false);
			//StartCoroutine (FadeAway ());
		}
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
}
