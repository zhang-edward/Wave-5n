using PlayerActions;
using UnityEngine;
using System.Collections;

public class Knight_ThornShield : HeroPowerUp {

	public PA_AreaEffect areaAttack;
	public SimpleAnimationPlayer anim;

	private KnightHero knight;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		this.knight = (KnightHero)hero;
		areaAttack.Init(hero.player, AreaAttackEffect);
		knight.OnKnightShieldHit += ExecuteAreaAttack;
	}

	private void ExecuteAreaAttack(IDamageable src)
	{
		areaAttack.SetPosition(knight.transform.position);
		areaAttack.Execute();
		anim.Play();
	}

	private void AreaAttackEffect(Enemy e) {
		knight.DamageEnemy(e, (int)(knight.damage * 0.5f), knight.hitEffect, false, knight.hitSounds);
	}
}
