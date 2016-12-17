using UnityEngine;
using System.Collections;

public class BurnStatus : EnemyStatus
{
	public float fireSpreadRange = 1.5f;
	public int numSpreads = 2;

	protected override IEnumerator Effect ()
	{
		while (timer >= 0)
		{
			// if this status can still spread
			if (numSpreads > 0)
			{
				// get a list of all enemies in a range
				Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, fireSpreadRange);
				foreach (Collider2D col in cols)
				{
					if (col.CompareTag ("Enemy"))
					{
						Enemy e = col.GetComponentInChildren<Enemy> ();
						// check if the enemy already has this status
						bool hasBurnStatus = false;
						foreach (EnemyStatus status in e.statuses)
						{
							if (status as BurnStatus != null)
							{
								hasBurnStatus = true;
							}
						}
						// if not, add a burn status but with no spread
						if (!hasBurnStatus)
						{
							GameObject toAdd = Instantiate (this.gameObject);
							toAdd.GetComponent<BurnStatus> ().numSpreads = 0;
							e.AddStatus (toAdd);
							numSpreads--;
						}
					}
				}
			}
			yield return null;
		}
		enemy.Damage (1);
		enemy.statuses.Remove (this);
		Destroy (gameObject);
		yield return null;
	}
}

