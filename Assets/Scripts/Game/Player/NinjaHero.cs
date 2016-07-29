using UnityEngine;
using System.Collections;

public class NinjaHero : PlayerHero {

	[Header("Class-Specific")]
	public NinjaSmokeBomb smokeBomb;
	public int smokeBombRange = 2;
	public Sprite hitEffect;

	public bool smokeScreened = false;
	//public int damage = 1;

	public override void Init(Player player, EntityPhysics body, Animator anim)
	{
		base.Init (player, body, anim);
		StartCoroutine (UpdateProperties());
	}

	private IEnumerator UpdateProperties()
	{
		while (true)
		{
			if (Vector3.Distance (transform.position, smokeBomb.transform.position) < smokeBombRange &&
				smokeBomb.gameObject.activeInHierarchy)
				smokeScreened = true;
			else
				smokeScreened = false;
			player.isInvincible = smokeScreened;
			yield return null;
		}
	}

	public override void Ability()
	{
		// if cooldown has not finished
		if (abilityCooldown > 0)
			return;
		abilityCooldown = cooldownTime;
		smokeBomb.Init (transform.position);
		Invoke ("ResetAbility", 5f);
	}

	public override void AbilityHoldDown ()
	{}

	public override void ResetAbility()
	{
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Enemy"))
		{
			if (smokeScreened)
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				if (!e.invincible && e.health > 0)
				{
					e.Damage (damage);
					/*Instantiate (hitEffect, 
						Vector3.Lerp (transform.position, e.transform.position, 0.5f), 
						Quaternion.identity);*/
					player.effectPool.GetPooledObject().GetComponent<TempObject>().Init(
						Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360f))),
						Vector3.Lerp (transform.position, e.transform.position, 0.5f), 
						hitEffect,
						true,
						0);

					player.TriggerOnEnemyDamagedEvent(damage);
				}
			}
		}
	}
}
