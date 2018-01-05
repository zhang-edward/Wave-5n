using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CollisionDetector), typeof(CircleCollider2D))]
public class EnemyDetectionZone : MonoBehaviour
{
	public CollisionDetector collision;
	public float lifetime;

	public delegate void OnDetectEnemyCallback(Enemy e);
	public OnDetectEnemyCallback onDetectEnemy;

	void Start()
	{
		collision.OnTriggerEnter += HandleCollideWithEnemy;
		Destroy (gameObject, lifetime);
	}

	public void SetOnDetectEnemyCallback(OnDetectEnemyCallback onDetectEnemy)
	{
		print(onDetectEnemy);
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
					onDetectEnemy(e);
			}
		}
	}
}

