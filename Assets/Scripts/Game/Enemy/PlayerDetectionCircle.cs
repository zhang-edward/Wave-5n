using UnityEngine;

public class EnemyHitZone : MonoBehaviour
{
	public float radius;

	public Player Activate()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Player"))
			{
				return col.GetComponentInChildren<Player>();
			}
		}
		return null;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, radius);
	}
}