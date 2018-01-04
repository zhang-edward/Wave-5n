using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Knight_FlyingSwords : HeroPowerUp
{
	private KnightHero knight;
	private int numSwords;

	private float addSwordChance = 0.05f;
	private float swordAttackChance = 1f;
	private int maxSwords = 3;

	public IndicatorEffect indicator;
	[Header("Animations")]
	public SimpleAnimation addSwordAnim;
	public SimpleAnimation swordAttackAnim;
	[Header("Audio")]
	public AudioClip portalOpenSound;
	public AudioClip swordRiseSound;
	public AudioClip[] swordLandSounds;


	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		knight = (KnightHero)hero;
		knight.OnKnightShieldHit += AddSword;
		knight.player.OnEnemyLastHit += AttackSword;
		percentActivated = 0;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		knight.OnKnightShieldHit -= AddSword;
		knight.player.OnEnemyLastHit -= AttackSword;
		numSwords = 0;
	}

	public override void Stack()
	{
		base.Stack();
		addSwordChance += 0.05f;
	}

	private void AddSword()
	{
		StartCoroutine(AddSwordRoutine());
	}

	private void AttackSword(Enemy e)
	{
		if (numSwords > 0)
			StartCoroutine(AttackSwordRoutine(e));
	}

	private IEnumerator AddSwordRoutine()
	{
		numSwords += 1;
		numSwords = Mathf.Min(numSwords, maxSwords);    // Cap numSwords to maxSwords (cannot go higher than max)

		Vector3 effectPos = UtilMethods.RandomOffsetVector2(transform.position, 2f);
		EffectPooler.PlayEffect(addSwordAnim, effectPos, false, 0.3f);
		SoundManager.instance.RandomizeSFX(portalOpenSound);

		float frame7time = addSwordAnim.SecondsPerFrame * 7f; // time before the sword rises up in the animation
		yield return new WaitForSeconds(frame7time);

		indicator.gameObject.SetActive(true);
		indicator.SetAnimatingOut(false);
		percentActivated = (float)numSwords / maxSwords;

		SoundManager.instance.RandomizeSFX(swordRiseSound);
	}

	private IEnumerator AttackSwordRoutine(Enemy e)
	{
		// if enemy is dead, do not activate
		if (!e.gameObject.activeInHierarchy)
			yield break;
		// this tests if multiple enemies were hit in the same frame and there is 1 sword left 
		if (numSwords <= 0)
			yield break;		
		numSwords--;
		float delay = Random.Range(0, 0.5f);					 // delay before the animation starts
		float frame6time = swordAttackAnim.SecondsPerFrame * 6f; // time before the sword hits the ground in the animation
		StunEnemy(e, delay + frame6time);

		yield return new WaitForSeconds(delay);

		EffectPooler.PlayEffect(swordAttackAnim, e.transform.position, false, 0.3f);
		SoundManager.instance.RandomizeSFX(portalOpenSound);

		yield return new WaitForSeconds(frame6time);

		e.Damage(knight.damage * 2);
		CameraControl.instance.StartShake(0.2f, 0.05f, true, false);
		SoundManager.instance.RandomizeSFX(swordLandSounds[Random.Range(0, swordLandSounds.Length)]);

		percentActivated = (float)numSwords / maxSwords;
		if (numSwords <= 0)
			indicator.AnimateOut();
	}

	private void StunEnemy(Enemy e, float time)
	{
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = time;
		e.AddStatus(stun.gameObject);
	}
}

