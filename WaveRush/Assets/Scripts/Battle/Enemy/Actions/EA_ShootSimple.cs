namespace EnemyActions
{
	using UnityEngine;
	using Projectiles;
	using System.Collections;

	public class EA_ShootSimple : EA_Shoot
	{
		[Header("Projectile Properties")]
		public float speed;
		public int damage;
		public Vector2 size;
		public SimpleAnimation projectileAnimation;

		protected override void Action()
		{
			anim.Play(actionState);     // triggers are unreliable, crossfade forces state to execute
			SoundManager.instance.RandomizeSFX(actionSound);

			Projectile p = projectilePool.GetPooledObject().GetComponent<Projectile>();
			UnityEngine.Assertions.Assert.IsNotNull(p);
			p.size = size;
			p.speed = speed;
			p.GetComponentInChildren<DamageAction>().damage = damage;
			p.projectileAnim = projectileAnimation;
			p.Init(shootPoint.position, dir, e);
		}
	}
}
