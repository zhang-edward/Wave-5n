namespace PlayerAbilities
{
	using UnityEngine;
	using System.Collections.Generic;

	public class AreaAttackAbility : PlayerAbility
	{
		private SoundManager sound;
		[Header("Properties")]
		public float range;				// The range of the attack
		public int maxHit;				// The maxmimum number of enemies that can be hit by the attack
		public float duration;          // the cooldown before the player can make his next action
		[Header("Effects and SFX")]
		public AudioClip areaAttackSound;
		public string areaAttackState = "Default";

		private List<Enemy> hitEnemies = new List<Enemy>(); // enemies collided with during one execution of this ability

		public delegate void HitEnemy(Enemy e);
		private HitEnemy OnHitEnemy;

		void Start()
		{
			sound = SoundManager.instance;
		}

		public void Init(Player player, HitEnemy onHitEnemyCallback)
		{
			base.Init(player);
			OnHitEnemy = onHitEnemyCallback;
		}

		public void Execute()
		{
			// Sound
			sound.RandomizeSFX(areaAttackSound);
			// Animation
			hero.anim.Play(areaAttackState);
			// Player properties
			player.isInvincible = true;
			player.input.isInputEnabled = false;
			hero.body.Move(Vector2.zero);

			int numEnemiesHit = 0;
			Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, range);
			foreach (Collider2D col in cols)
			{
				if (col.CompareTag("Enemy"))
				{
					numEnemiesHit++;
					if (numEnemiesHit < 5)
					{
						Enemy e = col.gameObject.GetComponentInChildren<Enemy>();
						hitEnemies.Add(e);
						if (OnHitEnemy != null)
							OnHitEnemy(e);
					}
				}
			}
			Invoke("Reset", 0.5f);
		}

		public void Reset()
		{
			player.input.isInputEnabled = true;
		}

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, range);
		}
	}
}
