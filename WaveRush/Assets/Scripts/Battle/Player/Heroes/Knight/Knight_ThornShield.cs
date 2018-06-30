using PlayerActions;
using UnityEngine;
using System.Collections;

public class Knight_ThornShield : HeroPowerUp {

	private const float STUN_DURATION = 2.0f;
	private const float STUN_RADIUS = 1.0f;

	public PA_AreaEffect stunArea;

	private KnightHero knight;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		this.knight = (KnightHero)hero;
		stunArea.Init(hero.player, AreaAttackEffect);
		knight.OnKnightShieldHit += ExecuteAreaAttack;
	}

	private void ExecuteAreaAttack(IDamageable src)
	{
		stunArea.SetPosition(knight.transform.position);
		stunArea.Execute();
	}

	private void AreaAttackEffect(Enemy e) {
		Vector2 awayFromPlayerDir = (e.transform.position - transform.position).normalized;
		e.body.AddImpulse(awayFromPlayerDir, 5f);
		e.Disable(0.5f);
		StartCoroutine(StunEnemyDelayed(e));
	}

	private void StunEnemy(Enemy e)
	{
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = 2.0f;
		e.AddStatus(stun.gameObject);
	}

	private IEnumerator StunEnemyDelayed(Enemy e)
	{
		yield return new WaitForSeconds(0.5f);
		StunEnemy(e);
	}
}
