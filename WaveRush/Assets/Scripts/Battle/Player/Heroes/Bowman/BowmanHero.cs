using UnityEngine;
using PlayerActions;

public class BowmanHero : PlayerHero {
	
	public const float PIERCING_ARROW_RANGE = 10f;

	public PA_CircleCast piercingArrowAbility;
	public PA_Move		 retreatAbility; 

	public GameObject arrowTrailPrefab;
	private ContinuousAnimatedLine arrowTrail;

	public override void Init (EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		base.Init (body, player, heroData);
		InitAbilities();
		arrowTrail = Instantiate(arrowTrailPrefab).GetComponent<ContinuousAnimatedLine>();
		// Handle input
		onTapHoldDown = () => { damageMultiplier = Mathf.Min(damageMultiplier + Time.deltaTime * 0.5f, 2.0f); };
		onTapHoldRelease = PiercingArrow;
		onTap = PiercingArrow;
		onDragRelease = Retreat;
	}

	private void InitAbilities() {
		piercingArrowAbility.Init(player, HandlePiercingArrowDamage);
		retreatAbility.Init(player);
	}

	private void PiercingArrow() {
		if (!CheckIfCooledDownNotify(0, true, HandleTap))
			return;
		ResetCooldownTimer(0);

		arrowTrail.Init(transform.position + (Vector3)player.dir.normalized, player.transform.position + (Vector3)player.dir.normalized * PIERCING_ARROW_RANGE);
		piercingArrowAbility.SetCast(player.transform.position, player.dir.normalized, PIERCING_ARROW_RANGE);
		piercingArrowAbility.Execute();
		damageMultiplier = 1;
	}

	public void HandlePiercingArrowDamage(Enemy e) {
		DamageEnemy(e, damage, null, false, null);
	}

	public void Retreat() {
		if (!CheckIfCooledDownNotify(1, true, HandleDragRelease))
			return;
		ResetCooldownTimer(1);

		retreatAbility.Execute();
	}

	public override void SpecialAbility() {
		print("Special ablity!");
	}

	protected override void ParryEffect(IDamageable src) {
		print("parried!");
	}

	public void DamageEnemy(Enemy e, int dmg, SimpleAnimation effect, bool tempSlowDown, AudioClip[] sfx)
	{
		if (!e.invincible && e.health > 0)
		{
			print ("dealt " + dmg + " dmg");
			e.Damage (dmg, player);
			// EffectPooler.PlayEffect(effect, e.transform.position, true, 0.1f);
			player.TriggerOnEnemyDamagedEvent(dmg);
			player.TriggerOnEnemyLastHitEvent (e);

			//sound.RandomizeSFX(sfx[Random.Range(0, sfx.Length)]);
			if (tempSlowDown)
				player.StartTempSlowDown(0.3f);
		}
	}

	protected override Quests.Quest UnlockQuest(HeroTier tier) {
		switch (tier) {
			case HeroTier.tier1:
				return null;
			case HeroTier.tier2:
				return null;
			case HeroTier.tier3:
				return null;	// TODO: Do this
			default:
				return null;
		}
	}
}
