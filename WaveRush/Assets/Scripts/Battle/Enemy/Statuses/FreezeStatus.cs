using UnityEngine;
using System.Collections;

public class FreezeStatus : EnemyStatus
{
	public ParticleSystem particles;
	public bool frozen;
	public AudioClip[] applySounds;


	protected override IEnumerator Effect ()
	{
		SoundManager.instance.RandomizeSFX (applySounds [Random.Range (0, applySounds.Length)]);
		// make the particle effect box emitter the same size as the entity 
		ParticleSystem.ShapeModule shapeModule = particles.shape;
		shapeModule.shapeType = ParticleSystemShapeType.Box;
		//shapeModule.scale = enemy.srSize * 0.5f;

		if (frozen)
		{
			enemy.Disable (timer);
			enemy.anim.enabled = false;
		}
		while (timer >= 0)
		{
			enemy.AddColor(Color.cyan);
			if (frozen)
				enemy.body.moveSpeed = 0;
			else
				enemy.body.moveSpeed = enemy.DEFAULT_SPEED / 2f;
			yield return null;
		}
		enemy.anim.enabled = true;
		enemy.RemoveColor(Color.cyan);
		enemy.body.moveSpeed = enemy.DEFAULT_SPEED;

		Deactivate();
	}

	public override void Stack ()
	{
		timer = duration;
		frozen = true;
		// restart coroutine
		StopAllCoroutines ();
		StartCoroutine ("Effect");
	}
}

