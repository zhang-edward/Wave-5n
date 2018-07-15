using UnityEngine;
using PlayerActions;

public class BowmanHero : PlayerHero {
	
	public const float PIERCING_ARROW_CHARGETIME_PER_LEVEL = 0.3f;
	public const float PIERCING_ARROW_RANGE = 10f;
	public const float CHARGE_DRAG = 3f;

	public PA_CircleCast piercingArrowAbility;
	public PA_Move		 retreatAbility; 

	public GameObject arrowTrailPrefab;
	private ContinuousAnimatedLine arrowTrail;

	private float piercingArrowCharge;
	private int piercingArrowChargeLevel;
	public bool charging;

#region Initialization
	public override void Init (EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		base.Init (body, player, heroData);
		InitAbilities();
		arrowTrail = Instantiate(arrowTrailPrefab).GetComponent<ContinuousAnimatedLine>();
		// Handle input
		onTapHoldDown = ChargePiercingArrow;
		onTap = PiercingArrow;
		onDragRelease = Retreat;
	}

	private void InitAbilities() {
		piercingArrowAbility.Init(player, HandlePiercingArrowDamage);
		retreatAbility.Init(player);
	}
#endregion
#region Abilities
	private void ChargePiercingArrow() {
		if (!CheckIfCooledDownNotify(0))
			return;

		piercingArrowCharge += Time.deltaTime;
		if (piercingArrowCharge >= PIERCING_ARROW_CHARGETIME_PER_LEVEL && piercingArrowChargeLevel < 2) {
			piercingArrowChargeLevel ++;
			piercingArrowCharge = 0;
		}
		switch (piercingArrowChargeLevel) {
			case 0:
				anim.Play("Charge1");
				break;
			case 1:
				anim.Play("Charge2");
				break;
			case 2:
				anim.Play("Charge3");
				break;
		}
		body.rb2d.drag = CHARGE_DRAG;
		// Set input listener on charge
		onDragRelease = null;
		onTouchEnded = PiercingArrow;
		onDragHold = ChargePiercingArrow;
		// Set drag indicator
		dragIndicatorEnabled = false;
	}

	private void PiercingArrow() {
		if (!CheckIfCooledDownNotify(0))
			return;
		ResetCooldownTimer(0);
		charging = false;
		// Set the body's direction
		body.Move(player.dir);
		body.Move(Vector2.zero);

		// Set damage multiplier
		damageMultiplier = Mathf.Lerp(1.0f, 2.0f, piercingArrowChargeLevel);
		// Effect
		arrowTrail.Init(transform.position + (Vector3)player.dir.normalized, player.transform.position + (Vector3)player.dir.normalized * PIERCING_ARROW_RANGE);
		// PA
		piercingArrowAbility.SetCast(player.transform.position, player.dir.normalized, PIERCING_ARROW_RANGE);
		piercingArrowAbility.Execute();
		// Reset properties
		damageMultiplier = 1;
		piercingArrowChargeLevel = 0;
		piercingArrowCharge = 0;
		// Set input listeners
		onDragRelease = Retreat;
		onDragHold = null;
		onTouchEnded = null;
		// Set drag indicator
		dragIndicatorEnabled = true;
		
	}

	public void HandlePiercingArrowDamage(Enemy e) {
		DamageEnemy(e, damage, null, false, null);
	}

	public void Retreat() {
		if (!CheckIfCooledDownNotify(1, true, HandleDragRelease))
			return;
		ResetCooldownTimer(1);

		anim.Play("Move");
		retreatAbility.Execute();
	}

	public override void SpecialAbility() {
		print("Special ablity!");
	}

	protected override void ParryEffect(IDamageable src) {
		print("parried!");
	}
#endregion
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

/** Unlock Quests */
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
