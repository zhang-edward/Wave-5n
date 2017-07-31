using UnityEngine;
using System.Collections;

public class MageFire : MonoBehaviour
{
	public SimpleAnimation hitEffect;

	public float radius;
	public float interval;
	public float lifetime;
	public int damage;

	void Start()
	{
		StartCoroutine (DamageEnemiesInRange ());
		Destroy (gameObject, lifetime);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, radius);
	}

	private IEnumerator DamageEnemiesInRange()
	{
		for (;;)
		{
			Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, radius);
			foreach (Collider2D col in cols)
			{
				if (col.CompareTag("Enemy"))
				{
					Enemy e = col.GetComponentInChildren<Enemy> ();
					if (!e.invincible)
					{
						e.Damage (damage);
						EffectPooler.PlayEffect(hitEffect, e.transform.position, true, 0.5f);
					}
				}
			}
			yield return new WaitForSeconds (interval);
		}
	}
}

