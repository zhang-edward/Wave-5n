using UnityEngine;
namespace Projectiles
{
	public class DamageAction : ProjectileAction
	{
		public int damage;

		void OnEnable()
		{
			projectile.OnCollidedTarget += DamageTarget;
		}

		void OnDisable()
		{
			projectile.OnCollidedTarget -= DamageTarget;
		}

		public void DamageTarget(IDamageable target)
		{
			projectile.DamageTarget(target, damage);
		}

		public override void Execute()
		{
		}
	}
}
