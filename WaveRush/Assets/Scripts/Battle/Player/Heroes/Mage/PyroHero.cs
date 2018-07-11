using UnityEngine;
using PlayerActions;
using Projectiles;
using System.Collections;

public class PyroHero : PlayerHero {

	/// <summary>
	/// Shoot a meteor projectile.
	/// NOTE: The target must be set with <see cref="PA_ShootProjectile.SetProjectileDirection"/> 
	/// before running <see cref="PlayerAction.Execute"/>.
	/// </summary>
	public class PA_ShootMeteorProjectile : PA_ShootProjectile {
		protected override void DoAction()
		{
			Vector3 origin = projectileDir + new Vector3(Random.Range(-3f, 3f), 10);
			SetProjectileOrigin(origin);
			SetProjectileDirection(projectileDir - origin);
			base.DoAction();
		}
	}
	[System.Serializable]
	public class PA_FireSpurt : PA_AreaEffect {
		private const float RANGE = 3.3f;

		public PA_Effect effect;
		public AudioClip[] fireSound;

		public override void Init(Player player, HitEnemy onHitEnemyCallback) {
			base.Init(player, onHitEnemyCallback);
			effect.Init(player);
		}

		public void SetDirection(Vector3 dir) {
			Vector3 position = player.transform.position + (dir.normalized * RANGE);
			SetPosition(position);
		}

		protected override void DoAction() {
			base.DoAction();
			effect.SetPosition(player.transform.position);
			effect.Execute();
			SoundManager.instance.PlaySingle(fireSound[Random.Range(0, fireSound.Length)]);
		}

		protected override void GetEnemiesHit() {
			base.GetEnemiesHit();
			RaycastHit2D[] hits = Physics2D.CircleCastAll(player.transform.position, 0.3f, player.dir, RANGE);
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.CompareTag("Enemy"))
				{
					Enemy e = hit.collider.GetComponentInChildren<Enemy>();
					if (!hitEnemies.Contains(e))
						hitEnemies.Add(e);
				}
			}
			hitEnemies.Shuffle();		// Mix the linecasted enemies with the overlap circle enemies
		}
	}

	private const float FIRESPURT_RECOIL = 3.0f;
	public  const float FIREZONE_RANGE = 4.0f;
	private const float SPECIAL_NUM_METEORS = 10f;

	private RuntimeObjectPooler fireballPool;
	private RuntimeObjectPooler meteorPool;

	[Header("Abilities")]
	//public PA_Rush specialRushAbility;
	public PA_FireSpurt	 			fireSpurtAbility;
	public PA_SpecialAbilityEffect  specialAbilityEffect;
	public PA_AreaEffect 			teleportAreaEffect;
	public PA_ShootMeteorProjectile shootMeteorAbility = new PA_ShootMeteorProjectile();
	public PA_Teleport teleportAbility;
	[Header("Prefabs")]
	public SimpleAnimation hitEffect;
	public SimpleAnimation specialHitEffect;
	public GameObject projectilePrefab;
	public GameObject meteorPrefab;
	[Header("Audio")]
	public AudioClip specialHitSound;
	public AudioClip shootSound;
	public AudioClip teleportOutSound;
	public AudioClip teleportInSound;
	public AudioClip powerUpSound;
	public AudioClip transformSound;
	public AudioClip powerDownSound;
	public AudioClip spawnMeteorSound;
	[Header("Etc")]
	public Map map;
	public AnimationSet magmaFormAnim;
	public SimpleAnimationPlayer transformEffect;
	public IndicatorEffect teleportIndicator;

	/** Private members */
	private bool  specialActivated;
	private float specialRushCooldown = 0.5f;
	private float specialRushCooldowntimer = 0;
	private float fireSpurtBuffer;
	private bool  shootStateSwitch;

	public delegate void PyroEvent();
	public event PyroEvent OnFireSpurtDamagedEnemy;
	public event PyroEvent OnPyroTeleportDamagedEnemy;
	public event PyroEvent OnParriedEnemy;
	public event PyroEvent OnMageTeleportOut;
	public event PyroEvent OnMageSpecialAbility;

	public Coroutine specialAbilityChargeRoutine;

	public delegate void MageCreatedObject (GameObject o);
	public event MageCreatedObject OnMageShotFireball;

#region Initialization
	public override void Init(EntityPhysics body, Player player, Pawn heroData)
	{
		cooldownTimers = new float[2];
		map = GameObject.Find ("Map").GetComponent<Map>();
		fireballPool = (RuntimeObjectPooler)projectilePrefab.GetComponent<Projectile>().GetObjectPooler();
		meteorPool = (RuntimeObjectPooler)meteorPrefab.GetComponent<Projectile>().GetObjectPooler();

		base.Init (body, player, heroData);

		onDragHold = ShootFireSpurt;
		onDragRelease = () => { fireSpurtBuffer = 0; };
		onTap = StartTeleport;
		InitAbilities();
	}

	private void InitAbilities()
	{
		fireSpurtAbility.Init(player, DamageEnemyFireSpurt);
		specialAbilityEffect.Init(player);
		shootMeteorAbility.Init(player, meteorPool);
		teleportAreaEffect.Init(player, TeleportDamageEnemy);
		//specialRushAbility.Init(player, DamageEnemySpecial);
		teleportAbility.Init(player);
		teleportAbility.OnTeleportIn += AreaAttack;
	}
#endregion
#region FireSpurt
	private void ShootFireSpurt()
	{
		if (fireSpurtBuffer < 0.2f)	{
			fireSpurtBuffer += Time.deltaTime;
			return;
		}
		if (cooldownTimers[0] > 0)
			return;
		ResetCooldownTimer (0);

		fireSpurtAbility.SetDirection(player.dir);
		fireSpurtAbility.Execute();
		if (shootStateSwitch)
			anim.Play("Shoot1");
		else
			anim.Play("Shoot2");
		shootStateSwitch = !shootStateSwitch;
		/** Recoil */
		Vector2 dir = player.dir.normalized;
		body.Move (dir);	// set the sprites flipX to the correct direction
		body.rb2d.velocity = dir * -FIRESPURT_RECOIL;

		/** Event */
		// if (OnMageShotFireball != null)
		// 	OnMageShotFireball (fireball.gameObject);
	}

	private void DamageEnemyFireSpurt(Enemy e) {
		DamageEnemy(e, damage);
		if (OnFireSpurtDamagedEnemy != null)
			OnFireSpurtDamagedEnemy();
	}
#endregion
#region Teleport
	private bool CanTeleport(Vector3 dest) {
		return map.WithinOpenCells(dest);
	}

	private void DisableTeleportIndicator() {
		teleportIndicator.gameObject.SetActive(false);
	}

	public void StartTeleport() {
		if (!CheckIfCooledDownNotify (1) || !CanTeleport(player.transform.position + (Vector3)player.dir))
			return;
		player.body.Move(Vector3.zero);		// Kill momentum
		player.dir = Vector2.ClampMagnitude(player.dir, FIREZONE_RANGE);
		teleportAbility.OnTeleportIn += DisableTeleportIndicator;
		teleportIndicator.gameObject.SetActive(true);
		teleportIndicator.transform.position = transform.position + (Vector3)player.dir;
		teleportAbility.SetDestination(transform.position + (Vector3)player.dir);
		teleportAbility.Execute();
		ResetCooldownTimer(1);
	}

	private void TeleportDamageEnemy(Enemy e) {
		DamageEnemy(e, damage);
		PushBackEnemy(e, 10f);
		if (OnPyroTeleportDamagedEnemy != null)
			OnPyroTeleportDamagedEnemy();
	}

	private void AreaAttack() {
		teleportAreaEffect.SetPosition(transform.position);
		teleportAreaEffect.Execute();
	}
#endregion
#region Parry
	protected override void ParryEffect(IDamageable src) {
		body.AddRandomImpulse();
		Enemy e = src as Enemy;
		if (e != null) {
			PushBackEnemy(e, 4f);
		}
		if (OnParriedEnemy != null)
			OnParriedEnemy();
	}
#endregion
#region SpecialAbility
	public override void SpecialAbility()
	{
		if (specialAbilityCharge < SPECIAL_ABILITY_CHARGE_CAPACITY)
			return;
		StartCoroutine(SpecialAbilityRoutine());
	}

	private IEnumerator SpecialAbilityRoutine()
	{
		sound.RandomizeSFX(powerUpSound);
		specialAbilityEffect.Execute();
		yield return new WaitForSeconds(specialAbilityEffect.duration);
		for (int i = 0; i < SPECIAL_NUM_METEORS; i ++)
		{
			Vector3 randomMapPosition = new Vector3						// Random map position with a buffer of 5 units
				(Random.Range(5, Map.size - 5), Random.Range(5, Map.size - 5));
			shootMeteorAbility.SetProjectileDirection(randomMapPosition);
			shootMeteorAbility.Execute();
			shootMeteorAbility.OnExecutedAction += SetMeteorProperties;
			yield return new WaitForSeconds(Random.Range(0.5f, 1f));
		}
	}

	private void SetMeteorProperties()
	{
		Projectile meteor = shootMeteorAbility.GetProjectile();
		meteor.GetComponentInChildren<AreaDamageAction>().damage = Mathf.RoundToInt(damage);
		meteor.OnDie += MeteorImpactEvent;
		specialAbilityCharge = 0;
	}

	private void MeteorImpactEvent()
	{
		CameraControl.instance.StartShake(0.3f, 0.1f, true, false);
		CameraControl.instance.StartFlashColor(Color.white, 0.2f, 0, 0, 0.2f);
	}
#endregion
#region Affect Enemy Methods
	// Damage an enemy and spawn an effect
	private void DamageEnemy(Enemy e, int amt)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage, player);
			if (specialActivated)
				EffectPooler.PlayEffect(specialHitEffect, e.transform.position, true);
			else
				EffectPooler.PlayEffect(hitEffect, e.transform.position, true, 0.2f);

			player.TriggerOnEnemyDamagedEvent(amt);
			player.TriggerOnEnemyLastHitEvent (e);
		}
	}

	private void PushBackEnemy(Enemy e, float strength) {
		Vector2 awayFromPlayerDir = (e.transform.position - transform.position).normalized;
		e.Disable(1.0f);
		e.body.AddImpulse(awayFromPlayerDir, strength);
	}
#endregion
#region Etc Methods
	public void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere(transform.position, teleportAreaEffect.range);
	}

	protected override void Update()
	{
		base.Update();
		if (specialRushCooldowntimer > 0)
			specialRushCooldowntimer -= Time.deltaTime;
	}
#endregion

	protected override Quests.Quest UnlockQuest(HeroTier tier) {
		switch (tier) {
			case HeroTier.tier1:
				return new Quests.CompleteStageQuest(GameManager.instance, 0, 1);
			case HeroTier.tier2:
				return null;
			case HeroTier.tier3:
				return null;	// TODO: Do this
			default:
				return null;
		}	
	}
}
