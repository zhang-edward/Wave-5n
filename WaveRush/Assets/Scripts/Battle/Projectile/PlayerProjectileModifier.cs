using UnityEngine;
namespace Projectiles
{
	public class PlayerProjectileModifier : MonoBehaviour
	{
		private Player player;
		private Projectile projectile;

		void Awake()
		{
			projectile = GetComponent<Projectile>();
		}

		void Start()
		{
			player = GameObject.Find("/Game/Player").GetComponentInChildren<Player>();
		}

		public void TriggerPlayerEvent(IDamageable target, int damage)
		{
			player.TriggerOnEnemyDamagedEvent(damage);
			player.TriggerOnEnemyLastHitEvent((Enemy)target);
		}

		void OnEnable()
		{
			projectile.OnDamagedTarget += TriggerPlayerEvent;
		}

		void OnDisable()
		{
			projectile.OnDamagedTarget -= TriggerPlayerEvent;
		}
	}
}
