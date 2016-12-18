using UnityEngine;
using System.Collections;

public class FreezeStatus : EnemyStatus
{
	public ParticleSystem particles;
	public bool frozen;

	protected override IEnumerator Effect ()
	{
		// make the particle effect box emitter the same size as the entity 
		ParticleSystem.ShapeModule shapeModule = particles.shape;
		shapeModule.shapeType = ParticleSystemShapeType.Box;
		shapeModule.box = enemy.srSize * 0.5f;

		if (frozen)
		{
			enemy.Disable (timer);
			enemy.anim.enabled = false;
		}
		while (timer >= 0)
		{
			enemy.sr.color = Color.cyan;
			if (frozen)
				enemy.body.moveSpeed = 0;
			else
				enemy.body.moveSpeed = enemy.DEFAULT_SPEED / 2f;
			yield return null;
		}
		enemy.anim.enabled = true;
		enemy.sr.color = Color.white;
		enemy.body.moveSpeed = enemy.DEFAULT_SPEED;

		enemy.statuses.Remove (this);
		Destroy (gameObject);
	}

	public override void Boost ()
	{
		timer = duration;
		frozen = true;
		// restart coroutine
		StopAllCoroutines ();
		StartCoroutine ("Effect");
	}
}

