using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	private float DEFAULT_SPEED;

	public PlayerInput input;
	public EntityPhysics body;
	public SpriteRenderer sr;
	public Animator anim;

	public bool killBox = false;

	void Start()
	{
		DEFAULT_SPEED = body.moveSpeed;
	}

	public void PrimaryAbility()
	{
		killBox = true;
		body.moveSpeed = 10;
		//sr.color = new Color (1, 0, 0);
		input.mouseMovement = false;
		anim.SetBool ("Attacking", true);
		Invoke ("ResetPrimaryAbility", 0.3f);
	}

	private void ResetPrimaryAbility()
	{
		killBox = false;
		body.moveSpeed = DEFAULT_SPEED;
		//sr.color = new Color (1, 1, 1);
		input.mouseMovement = true;
		anim.SetBool ("Attacking", false);
	}

	void Update()
	{
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Enemy"))
		{
			if (killBox)
				col.gameObject.SetActive (false);
		}
	}
}

