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
		print ("Knight shield hit");
		areaAttack.SetPosition(knight.transform.position);
		areaAttack.Execute();
		anim.Reset();
		anim.Play();
	}

	private void AreaAttackEffect(Enemy e) {
		knight.PushEnemyBack(e, 5, 0.5f);
		knight.DamageEnemy(e, (int)(knight.damage * 2), knight.hitEffect, false, null);
	}
}
