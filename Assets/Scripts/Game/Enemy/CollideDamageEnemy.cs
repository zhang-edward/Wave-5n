using UnityEngine;
using System.Collections;

public class CollideDamageEnemy : Enemy {

	public int damage = 1;
	public float attackCooldown = 1f;
	private float cooldown;

	private float attackBuildUp = 1f;	// time for the player to be in contact with the enemy before the player is damaged
	private float buildUp;				// timer for attackBuildUp

	void Update()
	{
		if (cooldown > 0)
			cooldown -= Time.deltaTime;
	}

	protected override IEnumerator MoveState()
	{
		moveState = GetAssignedMoveState();
		while (true)
		{
			moveState.UpdateState ();
			yield return null;
		}
	}

	protected override void ResetVars ()
	{
		body.gameObject.layer = DEFAULT_LAYER;
		body.moveSpeed = DEFAULT_SPEED;
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			Player player = col.GetComponentInChildren<Player>();
			if (cooldown <= 0 && health > 0 && !hitDisabled && buildUp >= attackBuildUp)
			{
				player.Damage (damage);
				cooldown = attackCooldown;
			}
			else
			{
				buildUp += Time.deltaTime;
			}
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.CompareTag ("Player"))
		{
			buildUp = 0;
		}
	}
}
