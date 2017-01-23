using UnityEngine;
using System.Collections;

public class CollideDamageAction : EnemyAction
{
	private float cooldown;
	private float attackBuildUp = 0.2f;	// time for the player to be in contact with the enemy before the player is damaged
	private float buildUp;				// timer for attackBuildUp

	public int damage = 1;
	public bool activated;
	public float attackCooldown = 1f;

	public override void Execute ()
	{
		activated = true;
	}

	public override void Interrupt ()
	{
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (!activated)
			return;
		if (col.CompareTag("Player"))
		{
			Player player = col.GetComponentInChildren<Player>();
			if (cooldown <= 0 && e.health > 0 && !e.hitDisabled && buildUp >= attackBuildUp)
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

