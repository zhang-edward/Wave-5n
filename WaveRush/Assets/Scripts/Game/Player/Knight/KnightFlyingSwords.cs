using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightFlyingSwords : HeroPowerUp
{
	private const float RADIUS = 4f;

	private KnightHero knight;
	private ObjectPooler effectPool;
	private int numSwords;

	private float addSwordChance = 0.1f;
	private float swordAttackChance = 0.5f;
	private int maxSwords = 3;

	public SimpleAnimation addSwordAnim;
	public SimpleAnimation swordAttackAnim;

	void Awake()
	{
		effectPool = ObjectPooler.GetObjectPooler("Effect");
	}

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		knight = (KnightHero)hero;
		knight.player.OnEnemyLastHit += Activate;
		percentActivated = 0;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		knight.player.OnEnemyLastHit -= Activate;
		numSwords = 0;
	}

	private void Activate(Enemy e)
	{
		if (numSwords > 0)
		{
			if (Random.value < swordAttackChance)
				StartCoroutine(ActivateSwordRoutine(e));
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

		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, addSwordAnim.TimeLength - 0.3f, 0.3f, new Color(1, 1, 1, 0.8f));
		anim.anim = addSwordAnim;
		tempObj.Init(Quaternion.identity,
					 e.transform.position,
		             addSwordAnim.frames[0]);
		anim.Play();

		yield return new WaitForSeconds(frame7time);
		e.Damage(1);
		percentActivated = 1f;
	}

	private IEnumerator ActivateSwordRoutine(Enemy e)
	{
		float delay = Random.Range(0, 0.5f);					 // delay before the animation starts
		float frame6time = swordAttackAnim.SecondsPerFrame * 6f; // time before the sword hits the ground in the animation

		StunEnemy(e, delay + frame6time);

		yield return new WaitForSeconds(delay);

		numSwords--;
		if (!e.gameObject.activeInHierarchy)
			yield break;

		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, swordAttackAnim.TimeLength - 0.3f, 0.3f, new Color(1, 1, 1, 0.8f));
		anim.anim = swordAttackAnim;
		tempObj.Init(Quaternion.identity,
					 e.transform.position,
		             swordAttackAnim.frames[0]);
		anim.Play();

		yield return new WaitForSeconds(frame6time);

		e.Damage(4);
		CameraControl.instance.StartShake(0.2f, 0.05f);
		percentActivated = (float)numSwords / maxSwords;
	}

	private void StunEnemy(Enemy e, float time)
	{
		StunStatus stun = Instantiate(StatusEffectContainer.instance.GetStatus("Stun")).GetComponent<StunStatus>();
		stun.duration = time;
		e.AddStatus(stun.gameObject);
	}
}

