using UnityEngine;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

	public Transform player;
	public EntityPhysics body;
	public Animator anim;

	protected float DEFAULT_SPEED;
	public float detectPlayerRange = 2f;

	void Start()
	{
		DEFAULT_SPEED = body.moveSpeed;
		StartCoroutine ("MoveState");
	}

	protected bool isPlayerNearby()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, detectPlayerRange);
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
