﻿using UnityEngine;
using System.Collections;

public class BurnStatus : EnemyStatus
{
	public ParticleSystem particles;
	public float fireSpreadRange = 1.5f;
	public int spreadLevel = 2;
	public int numSpreads = 2;

	protected override IEnumerator Effect ()
	{
		// last spread level should not spread
		if (spreadLevel == 0)
			numSpreads = 0;
		// make the particle effect box emitter the same size as the entity 
		ParticleSystem.ShapeModule shapeModule = particles.shape;
		shapeModule.shapeType = ParticleSystemShapeType.Box;
		shapeModule.box = enemy.srSize * 0.5f;

		while (timer >= 0)
		{
			// if this status can still spread
			if (numSpreads > 0)
			{
				SpreadFire ();
			}
			yield return null;
		}
		enemy.Damage (1);
		enemy.statuses.Remove (this);
		Destroy (gameObject);
		yield return null;
	}

	public override void Stack ()
	{
		timer = duration;
		spreadLevel++;
	}

	private void SpreadFire()
	{
		// get a list of all enemies in a range
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, fireSpreadRange);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag ("Enemy"))
			{
				Enemy e = col.GetComponentInChildren<Enemy> ();
				// check if the enemy already has this status
				bool hasBurnStatus = e.GetStatus(this) != null;
				// if not, add a burn status but with no spread
				if (!hasBurnStatus)
				{
					GameObject toAdd = Instantiate (this.gameObject);
					toAdd.GetComponent<BurnStatus> ().spreadLevel = spreadLevel - 1;
					e.AddStatus (toAdd);
					numSpreads--;
				}
			}
		}
	}
}

