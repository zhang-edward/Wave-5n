using UnityEngine;
using System.Collections;

public class CollideDamageEnemy : Enemy {

	public int damage = 1;
	public float attackCooldown = 2f;
	private float cooldown;

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
			if (cooldown <= 0 && health > 0 && !hitDisabled)
			{
				player.Damage (damage);
				cooldown = attackCooldown;
			}
		}
	}
}
