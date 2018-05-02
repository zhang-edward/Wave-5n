using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CollisionDetector), typeof(CircleCollider2D))]
public class EnemyDetectionZone : MonoBehaviour
{
	public float lifetime;
	public delegate void OnDetectEnemyCallback(EnemyDetectionZone zone, Enemy e);

	private CollisionDetector collision;
	private OnDetectEnemyCallback onDetectEnemy;

	void Start()
	{
		collision = GetComponent<CollisionDetector>();
		collision.OnTriggerEnter += HandleCollideWithEnemy;
		Destroy (gameObject, lifetime);
	}

	public void SetOnDetectEnemyCallback(OnDetectEnemyCallback onDetectEnemy)
	{
		this.onDetectEnemy = onDetectEnemy;
	}

	private void HandleCollideWithEnemy(Collider2D col)
	{
		if (col.CompareTag("Enemy"))
		{
			Enemy e = col.GetComponentInChildren<Enemy>();
			if (!e.invincible)
			{
				if (onDetectEnemy != null)
					onDetectEnemy(this, e);
			}
		}
	}
}

