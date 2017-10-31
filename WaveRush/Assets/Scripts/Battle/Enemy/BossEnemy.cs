﻿using UnityEngine;
using System.Collections;

public class BossEnemy : Enemy
{
	protected EnemyManager enemyManager;
	[HideInInspector]
	public GameObject soulPickupPrefab;
	[Header("Boss Attributes")]
	public AudioClip deathSound;
	public bool dying { get; private set; }

	private SimpleAnimation deathEffect;
	private int numSouls;

	public override void Init (Vector3 spawnLocation, Map map, int level)
	{
		canBeDisabledOnHit = false;
		numSouls = SoulsFormula(level);
		base.Init (spawnLocation, map, level);
		enemyManager = GetComponentInParent<EnemyManager> ();
		deathEffect = enemyManager.bossDeathEffect;
		CameraControl.instance.secondaryFocus = this.transform;
	}

	public override void Die ()
	{
		print ("Boss dying");
		action.Interrupt();
		foreach (EnemyStatus status in statuses)
		{
			status.gameObject.SetActive (false);
		}
		foreach (Enemy e in enemyManager.Enemies)
		{
			if (e.isActiveAndEnabled && e as BossEnemy == null)
				e.Disable (4f);
		}
		StopAllCoroutines ();
		StartCoroutine ("DeathAnimation");
	}

	protected override IEnumerator MoveState()
	{
		if (dying || health <= 0)
		{
			print("Boss dying from moveState");
			Die();
			yield break;
		}
		yield return base.MoveState();
	}

	public override void Damage(int amt)
	{
		if (dying)
			return;
		base.Damage(amt);
	}

	private IEnumerator DeathAnimation()
	{
		print("Start dying");
		CameraControl cam = CameraControl.instance;
		anim.CrossFade ("Dead", 0f);
		while (OtherBossesDying())
			yield return null;
		dying = true;
		cam.SetFocus (this.transform);
		yield return new WaitForSeconds(1f);
		EffectPooler.PlayEffect(deathEffect, transform.position);
		float delay = deathEffect.SecondsPerFrame * 3f;
		yield return new WaitForSeconds(delay);
		SoundManager.instance.RandomizeSFX (deathSound);
		cam.StartShake (0.5f, 0.05f, false, true);
		cam.StartFlashColor (Color.white, 1, 0, 0, 1);
		dying = false;
		cam.secondaryFocus = null;
		cam.ResetFocus ();
		SpawnSouls();
		base.Die ();

		yield return null;
	}

	private void SpawnSouls()
	{
		for (int i = 0; i < numSouls; i++)
		{
			GameObject o = Instantiate(soulPickupPrefab, transform.position, Quaternion.identity) as GameObject;
			o.GetComponent<SoulPickup>().Init(player);
		}
	}

	private bool OtherBossesDying()
	{
		foreach (BossEnemy boss in enemyManager.bosses)
		{
			if (boss.dying && boss != this)
			{
				return true;
			}
		}
		return false;
	}

	private static int SoulsFormula(int level)
	{
		if (level < Pawn.T2_MIN_LEVEL)
			return 1;
		else if (level < Pawn.T3_MIN_LEVEL)
			return Random.Range(1, 3);
		else
			return Random.Range(2, 4);
	}
}

