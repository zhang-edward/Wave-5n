using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerActions;

public class Mage_TeleportBurst : HeroPowerUp
{
	private const float DISABLE_DURATION = 0.5f;
	private const float STUN_DURATION = 1.5f;
	private const float RADIUS = 1f;

	private PyroHero mage;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		mage = (PyroHero)hero;
		mage.OnPyroTeleportDamagedEnemy += ActivateEffect;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		mage.OnPyroTeleportDamagedEnemy -= ActivateEffect;
	}

	public override void Stack()
	{
		base.Stack();
	}

	private void ActivateEffect()
	{
		List<Enemy> enemies = new List<Enemy>();
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, RADIUS);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy>();
				Vector2 awayFromPlayerDir = (e.transform.position - transform.position).normalized;
				e.Disable(0.5f);
				e.body.AddImpulse(awayFromPlayerDir, 10f);
				StartCoroutine(StunEnemyDelayed(e));
			}
		}
	}

	private IEnumerator StunEnemyDelayed(Enemy e)
	{
		yield return new WaitForSeconds(DISABLE_DURATION);
		StunEnemy(e, STUN_DURATION);
	}

	private void StunEnemy(Enemy e, float time)
	{
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = time;
		e.AddStatus(stun.gameObject);
	}
}
