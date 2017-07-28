namespace PlayerAbilities
{
	using UnityEngine;
	using Projectiles;

	public class ShootProjectileAbility : PlayerAbility
	{
		private SoundManager sound;
		private ObjectPooler projectilePool;
		public string shootState;
		public AudioClip shootSound;

		void Start()
		{
			sound = SoundManager.instance;
		}

		public void Init(Player player, ObjectPooler projectilePool)
		{
			base.Init(player);
			this.projectilePool = projectilePool;
		}

		public Projectile ShootProjectile(Vector2 dir)
		{
			// Sound
			sound.RandomizeSFX(shootSound);
			// Animation
			hero.anim.Play(shootState);

			GameObject projectileObj = projectilePool.GetPooledObject();
			Projectile projectile = projectileObj.GetComponent<Projectile>();
			projectile.Init(transform.position, dir);

			return projectile;
		}

		public void SetProjectile(ObjectPooler projectilePool)
		{
			this.projectilePool = projectilePool;
		}
	}
}