using UnityEngine;
using System.Collections;

public class MageProjectile : Projectile
{
	private Player player;

	public void Init(Vector3 pos, Vector2 dir, Sprite sprite, string target, Player player, float speed = 4, int damage = 1)
	{
		base.Init (pos, dir, sprite, target, speed, damage);
		this.player = player;
	}

	protected override void OnTriggerEnter2D (Collider2D col)
	{
		if (col.CompareTag(target))
		{
			IDamageable damageableTarget = col.GetComponentInChildren<IDamageable> ();
			damageableTarget.Damage (damage);
			player.TriggerOnEnemyDamagedEvent (damage);

			gameObject.SetActive (false);
		}
	}
}

