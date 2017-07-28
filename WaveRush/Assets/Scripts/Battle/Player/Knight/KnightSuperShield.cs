using UnityEngine;
using PlayerAbilities;
using System.Collections;

public class KnightSuperShield : HeroPowerUp
{
	private KnightHero knight;
	private float activateChance = 0.2f;

	public AreaAttackAbility areaAttackAbility;
	public GameObject areaAttackEffect;
	public IndicatorEffect indicatorEffect;

	public PlayerHero.InputAction storedOnTap;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		areaAttackAbility.Init(hero.player, DamageEnemy);
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
			storedOnTap = knight.onTap;
			knight.onTap = SuperShield;
			percentActivated = 1f;
			indicatorEffect.gameObject.SetActive(true);
		}
	}

	public void SuperShield()
	{
		// check cooldown
		if (!knight.IsCooledDown(1))
			return;
		knight.ResetCooldownTimer(1);
		areaAttackEffect.SetActive(true);
		// Properties
		knight.areaAttackShieldOn = true;
		knight.player.isInvincible = true;
		areaAttackAbility.Execute();

		// Reset Ability
		Invoke("ResetInvincibility", 1.5f);

		knight.onTap -= SuperShield;
		knight.onTap += knight.AreaAttack;
		indicatorEffect.AnimateOut();
		percentActivated = 0f;
	}

	public void ResetInvincibility()
	{
		knight.areaAttackShieldOn = false;
		areaAttackEffect.GetComponent<IndicatorEffect>().AnimateOut();

		knight.player.sr.color = Color.white;
		if (!knight.specialActivated)
			knight.player.isInvincible = false;
	}

	private void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.AddStatus(Instantiate(StatusEffectContainer.instance.GetStatus("Stun")));
			knight.DamageEnemy(e);
		}
	}
}

