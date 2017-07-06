using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightFlyingSwords : HeroPowerUp
{
	private KnightHero knight;
	private ObjectPooler effectPool;
	private int numSwords;

	private float addSwordChance = 0.05f;
	private float swordAttackChance = 0.5f;
	private int maxSwords = 3;

	public IndicatorEffect indicator;
	[Header("Animations")]
	public SimpleAnimation addSwordAnim;
	public SimpleAnimation swordAttackAnim;
	[Header("Audio")]
	public AudioClip portalOpenSound;
	public AudioClip swordRiseSound;
	public AudioClip[] swordLandSounds;

	void Awake()
	{
		effectPool = ObjectPooler.GetObjectPooler("Effect");
	}

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		knight = (KnightHero)hero;
		knight.player.OnEnemyLastHit += ActivateEffect;
		percentActivated = 0;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		knight.player.OnEnemyLastHit -= ActivateEffect;
		numSwords = 0;
	}

	public override void Stack()
	{
		base.Stack();
		addSwordChance += 0.05f;
	}

	private void ActivateEffect(Enemy e)
	{
		if (numSwords > 0)
		{
			if (Random.value < swordAttackChance)
			{
				StartCoroutine(ActivateSwordRoutine(e));
			}
		}
		else
		{
			if (Random.value < addSwordChance)
				StartCoroutine(AddSwords(e));
		}
	}

	private IEnumerator AddSwords(Enemy e)
	{
		numSwords = maxSwords;

		float frame7time = addSwordAnim.SecondsPerFrame * 7f; // time before the sword rises up in the animation
		StunEnemy(e, frame7time);

		PlayEffect(addSwordAnim, e.transform.position, 0.3f);
		SoundManager.instance.RandomizeSFX(portalOpenSound);

		yield return new WaitForSeconds(frame7time);

		indicator.gameObject.SetActive(true);
		indicator.SetAnimatingOut(false);
		e.Damage(1);
		percentActivated = 1f;
		SoundManager.instance.RandomizeSFX(swordRiseSound);
	}

	private IEnumerator ActivateSwordRoutine(Enemy e)
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

		PlayEffect(swordAttackAnim, e.transform.position, 0.3f);
		SoundManager.instance.RandomizeSFX(portalOpenSound);

		yield return new WaitForSeconds(frame6time);

		e.Damage(knight.damage * 3);
		CameraControl.instance.StartShake(0.2f, 0.05f);
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

	private void PlayEffect(SimpleAnimation toPlay, Vector3 position, float fadeOutTime)
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, toPlay.TimeLength - fadeOutTime, fadeOutTime, new Color(1, 1, 1, 0.8f));
		anim.anim = toPlay;
		tempObj.Init(Quaternion.identity,
		             position,
		             toPlay.frames[0]);
		anim.Play();
	}
}

