using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

	protected string DEFAULT_STATE = "MoveState";
	protected float DEFAULT_SPEED;

	public SpriteRenderer sr;
	public Transform player;
	public EntityPhysics body;
	public Animator anim;

	public bool hitDisabled = false;

	public float playerDetectionRange = 2f;

	public Sprite deathSprite;
	public Sprite[] deathProps;
	public GameObject deathPropPrefab;

	public int health;


	void Start()
	{
		DEFAULT_SPEED = body.moveSpeed;
		StartCoroutine ("MoveState");
	}

	void Update()
	{
	}

	public void HitDisable(Vector2 dir, int damage)
	{
		// Stop all states
		StopAllCoroutines ();
		body.HitDisable (dir);
		health -= damage;

		if (health > 0)
			StartCoroutine (HitDisableState ());
		else
		{
			anim.enabled = false;
			sr.sprite = deathSprite;
			sr.color = new Color (1, 1, 1, 0.8f);
			transform.parent.gameObject.layer = LayerMask.NameToLayer ("NoCollide");
			transform.parent.rotation = Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360)));
			SpawnDeathProps ();
			//transform.rotation = Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360)));
		}
	}

	private IEnumerator HitDisableState()
	{
		hitDisabled = true;
		sr.color = Color.red;
		//Debug.Log ("Stopped all Coroutines");
		yield return new WaitForSeconds (0.2f);
		sr.color = Color.white;
		hitDisabled = false;

		UnityEngine.Assertions.Assert.IsTrue(anim.HasState(0, Animator.StringToHash("default")));
		anim.CrossFade ("default", 0f);

		ResetVars ();
		StartCoroutine (DEFAULT_STATE);
		yield return null;
	}

	protected bool PlayerInRange()
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

	// implement object pooling
	private void SpawnDeathProps()
	{
		foreach(Sprite sprite in deathProps)
		{
			GameObject o = Instantiate (deathPropPrefab, transform.position, Quaternion.identity) as GameObject;
			o.transform.SetParent (this.transform);
			o.GetComponent<SpriteRenderer> ().sprite = sprite;
			o.GetComponent<Rigidbody2D> ().AddForce (new Vector2 (Random.value, Random.value));
			o.GetComponent<Rigidbody2D> ().AddTorque (Random.Range(-8f, 8f));
		}
	}

	protected abstract void ResetVars();
	protected abstract IEnumerator MoveState();
}
