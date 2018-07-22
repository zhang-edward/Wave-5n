using UnityEngine;
using System.Collections;
using PlayerActions;

public class BowmanHero : PlayerHero {
	
	[System.Serializable]
	public class PA_ArrowRain : PA_Joint {

		private const int BOW_EFFECT_SHOOT_FRAME = 5;

		// Edit in inspector
		public GameObject arrowRainParticlesPrefab;
		public PA_AreaEffect areaEffect;
		public PA_EffectCallback shootEffect;
		public AudioClip bowAppearSound;
		public AudioClip shootSound;
		public AudioClip areaEffectSound;

		private SoundManager sound;
		private PA_InputListener inputListener = new PA_InputListener(); 
		private PA_SpecialAbilityEffect specialAbilityEffect = new PA_SpecialAbilityEffect();
		private PA_Sequencer arrowRainAbility = new PA_Sequencer();
		private GameObject arrowrainParticles;
		private Vector3 position;
		
		public void Init(Player player, PA_AreaEffect.HitEnemy onHitEnemyCallback) {
			sound = SoundManager.instance;
			actions = new PlayerAction[1];			

			// Input listener part of the action
			specialAbilityEffect.duration = 2.0f;
			specialAbilityEffect.Init(player, shootEffect);
			inputListener.duration = 2.0f;
			inputListener.input = PA_InputListener.InputType.Tap;
			inputListener.Init(player, ArrowRain);

			// Arrow rain part of the action (part that does damage)
			shootEffect.Init(player, BOW_EFFECT_SHOOT_FRAME);
			shootEffect.onFrameReached += ShootSound;
			areaEffect.Init(player, onHitEnemyCallback);
			arrowRainAbility.Init(player, shootEffect, areaEffect);

			// Initialize joint effect
			base.Init(player, specialAbilityEffect, inputListener);

			arrowrainParticles = Instantiate(arrowRainParticlesPrefab);
			arrowrainParticles.SetActive(false);
		}

		private void ArrowRain(Vector3 pos) {
			position = player.transform.position + pos;
			areaEffect.OnExecutedAction += AreaEffect;
			shootEffect.SetPosition(player.transform.position);
			areaEffect.SetPosition(position);
			arrowRainAbility.Execute();
			sound.RandomizeSFX(bowAppearSound);
			FinishAction();
		}

		private void AreaEffect() {
			arrowrainParticles.transform.position = position;
			arrowrainParticles.SetActive(true);
			areaEffect.OnExecutedAction -= AreaEffect;
			sound.RandomizeSFX(areaEffectSound);
		}

		private void ShootSound(int frame) {
			if (frame != BOW_EFFECT_SHOOT_FRAME)
				return;	
			sound.RandomizeSFX(shootSound);
		}
	}
	public const float PIERCING_ARROW_CHARGETIME_PER_LEVEL = 0.3f;
	public const float PIERCING_ARROW_RANGE = 10f;
	public const float CHARGE_DRAG = 3f;

	[Header("Actions")]
	public PA_CircleCast piercingArrowAbility;
	public PA_Move		 retreatAbility;
	public PA_ArrowRain	 arrowRainAbility;
	[Header("Prefabs")]
	public GameObject arrowTrailPrefab;
	private ContinuousAnimatedLine arrowTrail;
	[Header("Graphics")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation critEffect;
	[Header("Audio")]
	public AudioClip[] shootSounds;
	public AudioClip specialHitSound;
	public AudioClip specialChargeSound;
	
	private float piercingArrowCharge;
	private int piercingArrowChargeLevel;

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
		retreatAbility.OnActionFinished += anim.player.ResetToDefault;
		arrowRainAbility.Init(player, SpecialAbilityHitEnemy);
		arrowRainAbility.shootEffect.OnActionFinished += ResetSpecialAbility;
	}
#endregion
#region Piercing Arrow
	private void ChargePiercingArrow(Vector3 dir) {
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

	private void PiercingArrow(Vector3 dir) {
		if (!CheckIfCooledDownNotify(0))
			return;
		ResetCooldownTimer(0);
		// Set the body's direction
		body.Move(dir);
		body.Move(Vector2.zero);

		// Set damage multiplier
		damageMultiplier = Mathf.Lerp(1.0f, 2.0f, piercingArrowChargeLevel);
		// Effect
		arrowTrail.Init(transform.position + dir.normalized, player.transform.position + dir.normalized * PIERCING_ARROW_RANGE);
		// Sound
		sound.RandomizeSFX(shootSounds[Random.Range(0, shootSounds.Length)]);
		// PA
		piercingArrowAbility.SetCast(player.transform.position, dir, PIERCING_ARROW_RANGE);
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
		int dmg = damage;
		if (TryCriticalDamage(ref dmg)) {
			DamageEnemy(e, dmg, critEffect, false, null);
		}
		else {
			DamageEnemy(e, dmg, hitEffect, false, null);
		}
	}
#endregion
#region Retreat
	public void Retreat(Vector3 dir) {
		if (!CheckIfCooledDownNotify(1, HandleDragRelease, dir))
			return;
		ResetCooldownTimer(1);

		anim.Play("Move");
		retreatAbility.SetDirection(dir);
		retreatAbility.Execute();
	}
#endregion
#region SpecialAbility
	public override void SpecialAbility() {
		sound.RandomizeSFX(specialChargeSound);
		onTap -= PiercingArrow;
		onTapHoldDown -= ChargePiercingArrow;
		arrowRainAbility.Execute();
		arrowRainAbility.areaEffect.OnExecutedAction += () => { specialAbilityCharge = 0; };
	}

	private void ResetSpecialAbility()
	{
		onTap += PiercingArrow;
		onTapHoldDown += ChargePiercingArrow;
	}
	
	public void SpecialAbilityHitEnemy(Enemy e) {
		if (!e.invincible && e.health > 0)
		{
			int dmg = (int)(damage * 1.5f);
			StartCoroutine(DamageEnemyDelayed(e, dmg));
			// sound.RandomizeSFX(sfx[Random.Range(0, sfx.Length)]);
		}
	}

	private IEnumerator DamageEnemyDelayed(Enemy e, int dmg) {
		yield return new WaitForSeconds(Random.Range(0, 1.0f));
		DamageEnemy(e, dmg, critEffect, true, specialHitSound);
	}

#endregion
#region Parry
	protected override void ParryEffect(IDamageable src) {
		cooldownTimers[0] = 0;
		piercingArrowChargeLevel = 2;
	}
#endregion
#region Misc
	public void DamageEnemy(Enemy e, int dmg, SimpleAnimation effect, bool tempSlowDown, params AudioClip[] sfx)
	{
		if (!e.invincible && e.health > 0)
		{
			// print ("dealt " + dmg + " dmg");
			e.Damage (dmg, player);
			player.TriggerOnEnemyDamagedEvent(dmg);
			player.TriggerOnEnemyLastHitEvent (e);

			// Effect
			EffectPooler.PlayEffect(effect, e.transform.position, true, 0.1f);
			// Sound
			if (sfx != null)
				sound.RandomizeSFX(sfx[Random.Range(0, sfx.Length)]);
			// TempSlowDown
			if (tempSlowDown)
				player.StartTempSlowDown(0.15f);
		}
	}
#endregion

/** Unlock Quests */
	protected override Quests.Quest UnlockQuest(HeroTier tier) {
		switch (tier) {
			case HeroTier.tier1:
				return new Quests.CompleteStageQuest(GameManager.instance, 0, 2);
			case HeroTier.tier2:
				return null;
			case HeroTier.tier3:
				return null;	// TODO: Do this
			default:
				return null;
		}
	}
}
