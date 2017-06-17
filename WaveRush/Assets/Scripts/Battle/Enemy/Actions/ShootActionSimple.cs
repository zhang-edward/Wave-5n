using UnityEngine;
using Projectiles;
using System.Collections;

namespace EnemyActions
{
	public class ShootActionSimple : ShootAction
	{
		[Header("Projectile Properties")]
		public float speed;
		public int damage;
		public Vector2 size;
		public SimpleAnimation projectileAnimation;

		protected override void Shoot(Vector2 dir)
		{
			anim.CrossFade(shootState, 0f);     // triggers are unreliable, crossfade forces state to execute
			Projectile p = projectilePool.GetPooledObject().GetComponent<Projectile>();
			UnityEngine.Assertions.Assert.IsNotNull(p);
			p.size = size;
			p.speed = speed;
			p.GetComponentInChildren<DamageAction>().damage = damage;
			p.projectileAnim = projectileAnimation;
			p.Init(shootPoint.position, dir);
			SoundManager.instance.RandomizeSFX(shootSound);
		}
	}
}
