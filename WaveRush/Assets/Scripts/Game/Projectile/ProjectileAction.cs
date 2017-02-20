using UnityEngine;

namespace Projectiles
{
	public abstract class ProjectileAction : MonoBehaviour
	{
		protected Projectile projectile;

		public enum ProjectileEventTrigger {
			OnShoot,
			OnFlight,
			OnCollide
		}
		public ProjectileEventTrigger eventTrigger;

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
					projectile.OnFlight += Execute;
					break;
				case ProjectileEventTrigger.OnShoot:
					projectile.OnShoot += Execute;
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
					projectile.OnFlight -= Execute;
					break;
				case ProjectileEventTrigger.OnShoot:
					projectile.OnShoot -= Execute;
					break;
			}
		}

		public abstract void Execute();
	}
}
