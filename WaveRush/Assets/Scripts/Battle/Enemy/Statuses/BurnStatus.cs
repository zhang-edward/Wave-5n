using UnityEngine;
using System.Collections;

public class BurnStatus : EnemyStatus
{
	private const float DAMAGE_INTERVAL = 1f;

	public ParticleSystem particles;
	public float fireSpreadRange = 1.5f;
	public int spreadLevel = 2;
	public int numSpreads = 2;
	public int damage = 1;
	public AudioClip[] applySounds;

	protected override IEnumerator Effect ()
	{
		SoundManager.instance.PlayInterrupt (applySounds [Random.Range (0, applySounds.Length)]);
		yield return new WaitForSeconds (0.1f);
		// Last spread level should not spread
		if (spreadLevel == 0)
			numSpreads = 0;
		StartCoroutine(SpreadToNearbyEnemies());
		// Make the particle effect box emitter the same size as the entity 
		ParticleSystem.ShapeModule shapeModule = particles.shape;
		shapeModule.shapeType = ParticleSystemShapeType.Box;
		//shapeModule.scale = enemy.srSize * 0.5f;

		while (timer >= 0)
		{
			enemy.Damage(damage, false);
			yield return new WaitForSeconds(DAMAGE_INTERVAL);
		}

		Deactivate();
		yield return null;
	}

	private IEnumerator SpreadToNearbyEnemies()
	{
		while (timer >= 0)
		{
			// if this status can still spread
			if (numSpreads > 0)
			{
				// Get a list of all enemies in a range
				Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, fireSpreadRange);
				foreach (Collider2D col in cols)
				{
					if (col != null && col.CompareTag("Enemy"))
					{
						Enemy e = col.GetComponentInChildren<Enemy>();
						SpreadFire(e);
						yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
					}
				}
			}
			else
				yield break;
		}
	}

	public override void Stack ()
	{
		timer = duration;
		spreadLevel++;
	}

	private void SpreadFire(Enemy e)
	{
		// Check if the enemy already has this status
		bool hasBurnStatus = e.GetStatus(statusName) != null;
		// If not, add a burn status but with no spread
		if (!hasBurnStatus)
		{
			GameObject toAdd = Instantiate(this.gameObject);
			toAdd.GetComponent<BurnStatus>().spreadLevel = spreadLevel - 1;
			e.AddStatus(toAdd);
			numSpreads--;

		}
	}
}

