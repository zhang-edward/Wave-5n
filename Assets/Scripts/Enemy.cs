using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

	protected string DEFAULT_STATE = "MoveState";

	public SpriteRenderer sr;
	public Transform player;
	public EntityPhysics body;
	public Animator anim;

	public bool hitDisabled = false;

	protected float DEFAULT_SPEED;
	public float playerDetectionRange = 2f;

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
		StartCoroutine (HitDisableState ());
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

	protected abstract void ResetVars();
	protected abstract IEnumerator MoveState();
}
