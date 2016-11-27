using UnityEngine;
using System.Collections;

public class MageFire : MonoBehaviour
{
	private ObjectPooler effectPool;
	public Sprite hitEffect;

	void Start()
	{
		effectPool = ObjectPooler.GetObjectPooler ("Effect");
		StartCoroutine (DamageEnemiesInRange ());
		Destroy (gameObject, 8.0f);
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 3.0f);
	}

	private IEnumerator DamageEnemiesInRange()
	{
		while (true)
		{
			Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 3.0f);
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
			yield return new WaitForSeconds (2.0f);
		}
	}
}

