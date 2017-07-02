using UnityEngine;
using System.Collections;

public class KnightSuperShield : HeroPowerUp
{
	private KnightHero knight;
	private float activateChance = 0.2f;

	public float areaAttackRange;
	public GameObject shieldEffect;
	public AudioClip areaAttackSound;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		this.knight = (KnightHero)hero;
		knight.OnKnightShield += ActivateSuperShield;
		percentActivated = 0f;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		knight.OnKnightShield -= ActivateSuperShield;
	}

	public override void Stack()
	{
		base.Stack();
		activateChance += 0.08f;
	}

	private void ActivateSuperShield()
	{
		if (Random.value < activateChance)
		{
			knight.onTap -= knight.AreaAttack;
			knight.onTap += SuperShield;
			percentActivated = 1f;
		}
	}

	private void SuperShield()
	{
		// check cooldown
		if (!knight.IsCooledDown(1))
			return;
		knight.ResetCooldownTimer(1);
		// Sound
		SoundManager.instance.RandomizeSFX(areaAttackSound);
		// Animation
		knight.anim.Play("AreaAttack");
		// Effects
		shieldEffect.SetActive(true);
		// Properties
		knight.player.isInvincible = true;
		knight.player.input.isInputEnabled = false;
		knight.player.sr.color = new Color(0.8f, 0.8f, 0.8f);
		knight.body.Move(Vector2.zero);

		bool enemyHit = false;
		int numEnemiesHit = 0;
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, areaAttackRange);
		knight.damageMultiplier *= 2f;
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				numEnemiesHit++;
				if (numEnemiesHit < knight.maxHit)
				{
					Enemy e = col.gameObject.GetComponentInChildren<Enemy>();
					e.AddStatus(Instantiate(StatusEffectContainer.instance.GetStatus("Stun")));
					knight.DamageEnemy(e);
					enemyHit = true;
				}
			}
		}
		knight.damageMultiplier *= 0.5f;
		if (enemyHit)
			SoundManager.instance.RandomizeSFX(knight.hitSounds[Random.Range(0, knight.hitSounds.Length)]);

		// Reset Ability
		Invoke("ResetAreaAttackAbility", 0.5f);
		Invoke("ResetInvincibility", 1.5f);

		knight.onTap -= SuperShield;
		knight.onTap += knight.AreaAttack;
		percentActivated = 0f;
	}

	private void ResetAreaAttackAbility()
	{
		knight.ResetAreaAttackAbility();
	}

	private void ResetInvincibility()
	{
		shieldEffect.GetComponent<IndicatorEffect>().AnimateOut();

		knight.player.sr.color = Color.white;
		knight.player.isInvincible = false;	
	}
}

