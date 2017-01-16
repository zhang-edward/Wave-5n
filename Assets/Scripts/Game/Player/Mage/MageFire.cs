using UnityEngine;
using System.Collections;

public class MageFire : MonoBehaviour
{
	private ObjectPooler effectPool;
	public Sprite hitEffect;

	public float radius;
	public float interval;
	public float lifetime;

	void Start()
	{
		effectPool = ObjectPooler.GetObjectPooler ("Effect");
		StartCoroutine (DamageEnemiesInRange ());
		Destroy (gameObject, lifetime);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, radius);
	}

	private IEnumerator DamageEnemiesInRange()
	{
		while (true)
		{
			Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, radius);
			foreach (Collider2D col in cols)
			{
				if (col.CompareTag("Enemy"))
				{
					Enemy e = col.GetComponentInChildren<Enemy> ();
					if (!e.invincible)
					{
						e.Damage (1);
						effectPool.GetPooledObject ().GetComponent<TempObject> ().Init (
							Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360f))),
							e.transform.position, 
							hitEffect,
							true,
							0,
							0.2f,
							1.0f);
					}
				}
			}
			yield return new WaitForSeconds (interval);
		}
	}
}

