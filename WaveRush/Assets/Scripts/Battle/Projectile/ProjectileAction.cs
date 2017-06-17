using UnityEngine;

namespace Projectiles
{
	public abstract class ProjectileAction : MonoBehaviour
	{
		protected Projectile projectile;

		public enum ProjectileEventTrigger {
			OnShoot,
			OnFlight,
			OnCollide,
			OnDie
		}
		public ProjectileEventTrigger eventTrigger;
		public float interval;
		private float timer;

		void Awake()
		{
			projectile = GetComponentInParent<Projectile>();
		}

		void OnEnable()
		{
			switch (eventTrigger)
			{
				case ProjectileEventTrigger.OnCollide:
					projectile.OnCollide += Execute;
					break;
				case ProjectileEventTrigger.OnFlight:
					projectile.OnFlight += ExecuteInterval;
					break;
				case ProjectileEventTrigger.OnShoot:
					projectile.OnShoot += Execute;
					break;
				case ProjectileEventTrigger.OnDie:
					projectile.OnDie += Execute;
					break;
			}
		}

		void OnDisable()
		{
			switch (eventTrigger)
			{
				case ProjectileEventTrigger.OnCollide:
					projectile.OnCollide -= Execute;
					break;
				case ProjectileEventTrigger.OnFlight:
					projectile.OnFlight -= ExecuteInterval;
					break;
				case ProjectileEventTrigger.OnShoot:
					projectile.OnShoot -= Execute;
					break;
				case ProjectileEventTrigger.OnDie:
					projectile.OnDie -= Execute;
					break;
			}
		}

		private void ExecuteInterval()
		{
			timer += Time.deltaTime;
			if (timer >= interval)
			{
				Execute();
				timer = 0;
			}
		}

		public abstract void Execute();
	}
}
