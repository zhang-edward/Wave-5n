using UnityEngine;
using System.Collections;

public class ApplyStatusEffectPowerUp : HeroPowerUp
{
	[Header("ApplyStatus Properties")]
	public string status;

	[Header("Do not set in inspector")]
	public float applyStatusChance;

	public void Init (string status)
	{
		this.status = status;
	}

	public override void Activate (PlayerHero hero)
	{
		base.Activate (hero);
		applyStatusChance = 0.1f;
		hero.player.OnEnemyLastHit += ApplyStatus;
	}

	public override void Deactivate ()
	{
		base.Deactivate ();
		applyStatusChance = 0f;
	}

	public override void Stack ()
	{
		base.Stack ();
		applyStatusChance += 0.05f;
	}

	private void ApplyStatus(Enemy e)
	{
		if (Random.value < applyStatusChance)
			e.AddStatus (Instantiate (StatusEffectContainer.instance.GetStatus (status)));
	}
}

