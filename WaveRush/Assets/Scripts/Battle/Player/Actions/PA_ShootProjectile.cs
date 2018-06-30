namespace PlayerActions
{
	using UnityEngine;
	using Projectiles;

	[System.Serializable]
	public class PA_ShootProjectile : PlayerAction
	{
		private SoundManager sound;
		private ObjectPooler projectilePool;
		protected Vector3 projectileOrigin;
		protected Vector3 projectileDir;

		public string 	  shootState = "Default";
		public AudioClip  shootSound;

		public Projectile lastProjectileShot { get; private set; }


		public void Init(Player player, ObjectPooler projectilePool)
		{
			base.Init(player);
			this.projectilePool = projectilePool;
			sound = SoundManager.instance;
		}

		protected override void DoAction()
		{
			// Sound
			sound.RandomizeSFX(shootSound);
			// Animation
			hero.anim.Play(shootState);

			GameObject projectileObj = projectilePool.GetPooledObject();
			Projectile projectile = projectileObj.GetComponent<Projectile>();
			projectile.Init(projectileOrigin, projectileDir, player);

			lastProjectileShot = projectile;
		}

		public void SetProjectile(ObjectPooler projectilePool)
		{
			this.projectilePool = projectilePool;
		}

		public Projectile GetProjectile()
		{
			return lastProjectileShot;
		}

		public void SetProjectileOrigin(Vector3 pos)
		{
			projectileOrigin = pos;
		}

		public void SetProjectileDirection(Vector3 dir)
		{
			projectileDir = dir;
		}
	}
}