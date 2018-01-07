using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerActions;

public class Knight_FlyingSwords : HeroPowerUp
{
	public const int ADD_SWORD_FRAME    = 7;
	public const int ATTACK_SWORD_FRAME = 6;

	private KnightHero knight;
	private int numSwords;

	private float addSwordChance = 0.05f;
	private float swordAttackChance = 1f;
	private int maxSwords = 3;

	public IndicatorEffect indicator;
	[Header("Animations")]
	public PA_EffectCallback addSword;
	public PA_EffectCallback attackSword;
	private Queue<Enemy> enemiesToAttack = new Queue<Enemy>();
	//public SimpleAnimation addSwordAnim;
	//public SimpleAnimation swordAttackAnim;
	[Header("Audio")]
	public AudioClip portalOpenSound;
	public AudioClip swordRiseSound;
	public AudioClip[] swordLandSounds;


	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		knight = (KnightHero)hero;
		// Set hero event listeners
		knight.OnKnightShieldHit 	+= AddSword;
		knight.OnKnightRushHitEnemy += AttackSword;
		// Set animation frame listeners
		addSword.Init   (hero.player, ADD_SWORD_FRAME);
		attackSword.Init(hero.player, ATTACK_SWORD_FRAME);
		addSword.onFrameReached    += HandleOnAddSwordFrameReached;
		attackSword.onFrameReached += HandleOnAttackSwordFrameReached;
		// Set powerup properties
		percentActivated = 0;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		knight.OnKnightShieldHit 	-= AddSword;
		knight.OnKnightRushHitEnemy -= AttackSword;
		numSwords = 0;
	}

	public override void Stack()
	{
		base.Stack();
		addSwordChance += 0.05f;
	}

	private void AddSword()
	{
		// Set properties
		numSwords += 1;
		numSwords = Mathf.Min(numSwords, maxSwords);    // Cap numSwords to maxSwords (cannot go higher than max)
		// Execute animation
		Vector3 effectPos = UtilMethods.RandomOffsetVector2(transform.position, 2f);
		addSword.SetPosition(effectPos);
		addSword.Execute();
		// Sound
		SoundManager.instance.RandomizeSFX(portalOpenSound);
	}

	private void AttackSword(Enemy e)
	{
		enemiesToAttack.Enqueue(e);
		if (numSwords > 0)
			StartCoroutine(AttackSwordRoutine(e));
	}

	private void HandleOnAddSwordFrameReached(int frame)
	{
		if (frame != ADD_SWORD_FRAME)
		{
			Debug.LogError("PA_EffectCallback invoked the callback on the wrong frame!");
			return;
		}

		indicator.gameObject.SetActive(true);
		indicator.SetAnimatingOut(false);
		percentActivated = (float)numSwords / maxSwords;
		SoundManager.instance.RandomizeSFX(swordRiseSound);
	}

	void HandleOnAttackSwordFrameReached(int frame)
	{
		if (frame != ATTACK_SWORD_FRAME)
		{
			Debug.LogError("PA_EffectCallback invoked the callback on the wrong frame!");
			return;
		}
		Enemy e = enemiesToAttack.Dequeue();
		// If enemy is dead, do not attempt to damage it
		if (e.gameObject.activeInHierarchy)
			e.Damage(knight.damage * 2);
		CameraControl.instance.StartShake(0.2f, 0.05f, true, false);
		SoundManager.instance.RandomizeSFX(swordLandSounds[Random.Range(0, swordLandSounds.Length)]);

		percentActivated = (float)numSwords / maxSwords;
		if (numSwords <= 0)
			indicator.AnimateOut();
	}

	private IEnumerator AttackSwordRoutine(Enemy e)
	{
		// If enemy is dead, do not activate
		if (!e.gameObject.activeInHierarchy)
			yield break;
		// Test if multiple enemies were hit in the same frame and there is 1 sword left 
		if (numSwords <= 0)
			yield break;		
		numSwords--;
		float delay = Random.Range(0, 0.5f);						 // Delay before the animation starts
		float actionDelay = attackSword.effect.SecondsPerFrame * 6f; // Time before the sword hits the ground in the animation

		e.Disable(delay + actionDelay);

		yield return new WaitForSeconds(delay);

		attackSword.SetPosition(e.transform.position);
		attackSword.Execute();
		SoundManager.instance.RandomizeSFX(portalOpenSound);
	}

	private void StunEnemy(Enemy e, float time)
	{
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = time;
		e.AddStatus(stun.gameObject);
	}
}

