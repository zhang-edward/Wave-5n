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
	public const float PIERCING_ARROW_CHARGETIME_PER_LEVEL = 0.6f;
	public const float PIERCING_ARROW_RANGE = 10f;
	public const float CHARGE_DRAG = 3f;

	[Header("Actions")]
	public PA_Animate	 piercingArrowAnticipation;
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

	public delegate void BowmanHitEnemyEvent(Enemy e);
	public BowmanHitEnemyEvent OnPiercingArrow;

	public int piercingArrowChargeLevel { get; private set; }
	private float piercingArrowCharge;
	private bool specialActivated;

#region Initialization
	public override void Init (EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		base.Init (body, player, heroData);
		InitAbilities();
		arrowTrail = Instantiate(arrowTrailPrefab).GetComponent<ContinuousAnimatedLine>();
		// Handle input
		onTapHoldDown = ChargePiercingArrow;
		onTap = TapShoot;
		onDragRelease = Retreat;
		player.OnPlayerDamaged += InterruptCharge;
	}

	private void InitAbilities() {
		// Piercing Arrow Anticipation
		piercingArrowAnticipation.Init(player);
		piercingArrowAnticipation.OnActionFinished += () => { piercingArrowAbility.Execute(); };
		// Piercing Arrow
		piercingArrowAbility.Init(player, HandlePiercingArrowDamage);
		piercingArrowAbility.OnExecutedAction += () => {
			Vector3 dir = piercingArrowAbility.dir;
			arrowTrail.Init(transform.position + dir.normalized, player.transform.position + dir.normalized * PIERCING_ARROW_RANGE);
		};
		// Retreat
		retreatAbility.Init(player);
		retreatAbility.OnActionFinished += () => {
			if (anim.player.IsPlayingAnimation("Move") && body.rb2d.velocity.magnitude < body.moveSpeed)
				anim.player.ResetToDefault();
		};
		// Arrow Rain
		arrowRainAbility.Init(player, SpecialAbilityHitEnemy);
		arrowRainAbility.shootEffect.OnActionFinished += ResetSpecialAbility;
		arrowRainAbility.OnActionFinished += ResetSpecialAbility;
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
		if (!anim.player.IsPlayingAnimation("Charge"))
			anim.Play("Charge");
		player.sr.sprite = anim.GetAnimation("Charge").frames[piercingArrowChargeLevel];
		body.rb2d.drag = CHARGE_DRAG;
		// Set input listener on charge
		onDragRelease = null;
		onTouchEnded = Shoot;		// Want the "touch ended" dir, not the "drag release" dir
		onDragHold = ChargePiercingArrow;
		body.Move(Vector2.zero, 0);	// Cancel movement
		// Set drag indicator
		dragIndicatorEnabled = false;
	}

	private void InterruptCharge(int foo) {
		piercingArrowCharge = 0;
		piercingArrowChargeLevel = 0;
	}

	private void Shoot(Vector3 dir) {
		PiercingArrow(dir, true);
	}

	private void TapShoot(Vector3 dir) {
		PiercingArrow(dir, false);
	}

	private void PiercingArrow(Vector3 dir, bool charged, bool resetCooldown = true) {
		if (resetCooldown) {
			if (!CheckIfCooledDownNotify(0))
				return;
			ResetCooldownTimer(0);
		}
		// Set the body's direction
		body.Move(dir);
		body.Move(Vector2.zero, 0);

		// Set damage multiplier
		damageMultiplier = Mathf.Lerp(1.0f, 3.0f, (piercingArrowChargeLevel) - 1 / 2);
		piercingArrowAbility.SetCast(player.transform.position, dir, PIERCING_ARROW_RANGE);
		if (charged)
			piercingArrowAbility.Execute();
		else
			piercingArrowAnticipation.Execute();
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
		if (OnPiercingArrow != null)
			OnPiercingArrow(e);
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
		if (specialAbilityCharge < SPECIAL_ABILITY_CHARGE_CAPACITY || specialActivated)
			return;
		sound.RandomizeSFX(specialChargeSound);
		onTap -= TapShoot;
		onTapHoldDown -= ChargePiercingArrow;
		arrowRainAbility.Execute();
		arrowRainAbility.areaEffect.OnExecutedAction += () => { specialAbilityCharge = 0; };
		specialActivated = true;
	}

	private void ResetSpecialAbility() {
		if (!specialActivated)
			return;
		onTap += TapShoot;
		onTapHoldDown += ChargePiercingArrow;
		specialActivated = false;
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
		// cooldownTimers[0] = 0;
		// piercingArrowChargeLevel = 1;
		Vector3 dir = (transform.position - ((MonoBehaviour)src).transform.position).normalized * 4.0f;
		body.Move(dir, 0);
		anim.Play("Move");
		if (src as Enemy != null) {
			StartCoroutine(ParryEffectRoutine((Enemy)src));
		}
	}

	private IEnumerator ParryEffectRoutine(Enemy e) {
		yield return new WaitForSeconds(0.2f);
		PiercingArrow(e.transform.position - player.transform.position, true, false);
	}
#endregion
#region Misc
	public void DamageEnemy(Enemy e, int dmg, SimpleAnimation effect, bool tempSlowDown, params AudioClip[] sfx)
	{
		if (!e.invincible && e.health > 0)
		{
			// print ("dealt " + dmg + " dmg");
			e.Damage (dmg, player, true);
			player.TriggerOnEnemyDamagedEvent(dmg);
			player.TriggerOnEnemyLastHitEvent (e);

			// Effect
			EffectPooler.PlayEffect(effect, e.transform.position, true, 0.1f);
			// Sound
			if (sfx != null)
				sound.RandomizeSFX(sfx[Random.Range(0, sfx.Length)]);
			// TempSlowDown
			if (tempSlowDown)
				player.StartTempSlowDown(0.2f);
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
