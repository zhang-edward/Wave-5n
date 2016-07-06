using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

	protected string DEFAULT_STATE = "MoveState";

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

	public void hitDisable()
	{
		// Stop all states
		StopAllCoroutines ();
		StartCoroutine (HitDisableState ());
	}

	private IEnumerator HitDisableState()
	{
		hitDisabled = true;
		body.HitDisable ();
		//Debug.Log ("Stopped all Coroutines");
		yield return new WaitForSeconds (0.5f);
		hitDisabled = false;

		UnityEngine.Assertions.Assert.IsTrue(anim.HasState(0, Animator.StringToHash("idle")));
		anim.CrossFade ("idle", 0f);

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

	protected abstract IEnumerator MoveState();
}
