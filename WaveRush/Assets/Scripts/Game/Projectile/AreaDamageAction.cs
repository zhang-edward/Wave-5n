using UnityEngine;
namespace Projectiles
{
	public class AreaDamageAction : ProjectileAction
	{
		public float radius;
		public int damage;

		void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, radius);
		}

		public override void Execute()
		{
			Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
			foreach (Collider2D col in cols)
			{
				if (col.CompareTag(projectile.target))
				{
					IDamageable damageableTarget = col.GetComponentInChildren<IDamageable>();
					projectile.DamageTarget(damageableTarget, damage);
				}
			}
		}
	}
}
