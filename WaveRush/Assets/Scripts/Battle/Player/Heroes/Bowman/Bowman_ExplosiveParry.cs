using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerActions;

public class Bowman_ExplosiveParry : HeroPowerUp {

	public PA_AreaEffect areaEffect;
	private BowmanHero bowman;

	public override void Activate(PlayerHero hero) {
		base.Activate(hero);
		bowman = (BowmanHero)hero;
		bowman.onParrySuccess += AreaEffect;
		areaEffect.Init(hero.player, AreaEffectHitEnemy);
	}

	public override void Deactivate() {
		base.Deactivate();
	}

	private void AreaEffect() {
		areaEffect.SetPosition(bowman.transform.position);
		areaEffect.Execute();
	}

	private void AreaEffectHitEnemy(Enemy e) {
		PushEnemyBack(e, 5.0f, 0.5f);
		StartCoroutine(StunEnemyDelayed(e, 2.0f, 0.5f));
	}

	private IEnumerator StunEnemyDelayed(Enemy e, float stunTime, float delay) {
		yield return new WaitForSeconds(delay);
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = stunTime;
		e.AddStatus(stun.gameObject);
	}

	private void PushEnemyBack(Enemy e, float strength, float time) {
		Vector2 awayFromPlayerDir = (e.transform.position - bowman.transform.position).normalized;
		e.Disable(time);
		e.body.AddImpulse(awayFromPlayerDir, strength);
	}
}
