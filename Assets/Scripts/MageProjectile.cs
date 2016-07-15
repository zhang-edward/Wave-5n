using UnityEngine;
using System.Collections;

public class MageProjectile : Projectile
{
	private Player player;
	private bool outsideMapBounds;

	public void Init(Vector3 pos, Vector2 dir, Sprite sprite, string target, Player player, float speed = 4, int damage = 1)
	{
		base.Init (pos, dir, sprite, target, speed, damage);
		outsideMapBounds = false;
		this.player = player;
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 1.5f);
	}

	protected override void OnTriggerEnter2D (Collider2D col)
	{
		//Debug.Log (col.tag);
		if (col.CompareTag(target) && !outsideMapBounds)
		{
			Debug.Log (col.gameObject);
			Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 1.5f);
			foreach (Collider2D colChild in cols)
			{
				if (colChild.CompareTag(target))
				{
					IDamageable damageableTarget = colChild.GetComponentInChildren<IDamageable> ();
					damageableTarget.Damage (damage);
					player.TriggerOnEnemyDamagedEvent (damage);
				}
			}
			gameObject.SetActive (false);
		}
		else if (col.CompareTag("MapBorder"))
		{
			outsideMapBounds = true;	
		}
	}
}

