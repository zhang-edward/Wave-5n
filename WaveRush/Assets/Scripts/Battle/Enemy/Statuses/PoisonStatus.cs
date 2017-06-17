﻿using UnityEngine;
using System.Collections;

public class PoisonStatus : EnemyStatus
{
	public ParticleSystem particles;
	public float poisonInterval = 1f;
	public AudioClip[] applySounds;

	protected override IEnumerator Effect ()
	{
		SoundManager.instance.RandomizeSFX (applySounds [Random.Range (0, applySounds.Length)]);
		// make the particle effect box emitter the same size as the entity 
		ParticleSystem.ShapeModule shapeModule = particles.shape;
		shapeModule.shapeType = ParticleSystemShapeType.Box;
		shapeModule.box = enemy.srSize * 0.5f;

		// damage every 1 second
		InvokeRepeating("Poison", poisonInterval, poisonInterval);
		while (timer >= 0)
		{
			enemy.sr.color = Color.green;
			yield return null;
		}
		enemy.anim.enabled = true;
		enemy.sr.color = Color.white;
		enemy.body.moveSpeed = enemy.DEFAULT_SPEED;

		Deactivate();
	}

	private void Poison()
	{
		enemy.Damage (1);
	}

	void OnDisable()
	{
		CancelInvoke ();
	}

	public override void Stack ()
	{
		CancelInvoke ();
		timer = duration;
		poisonInterval /= 1.1f;
		// restart coroutine
		StopAllCoroutines ();
		StartCoroutine ("Effect");
	}
}

