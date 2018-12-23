﻿namespace PlayerActions
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	[System.Serializable]
	public class PA_Rush : PlayerAction
	{
		private SoundManager sound;
		[Header("Properties")]
		public PA_Move movement;
		public PA_EffectAttached effect;
		public CollisionDetector collision;
		public bool lockInput;
		public bool persistAnimation;
		public int maxHit;				// the maxmimum number of enemies that can be collided with during the rush
		[Header("Effects and SFX")]
		public string     rushState = "Default";
		public AudioClip  rushSound;
		private List<Enemy> hitEnemies = new List<Enemy>();	// enemies collided with during one execution of this ability
		private bool rushHitBoxOn;
		private Vector3 dir;

		public delegate void HitEnemy(Enemy e);
		private HitEnemy OnHitEnemy;

		public void Init(Player player, HitEnemy onHitEnemyCallback)
		{
			base.Init(player);
			movement.Init(player);
			effect.Init(player);
			OnHitEnemy = onHitEnemyCallback;
			sound = SoundManager.instance;
			collision.OnTriggerStay += HandleCollideWithEnemy;
		}

		public void SetDirection(Vector3 dir) {
			this.dir = dir;
		}

		protected override void DoAction()
		{
			player.StartCoroutine(RushRoutine());
		}

		private IEnumerator RushRoutine()
		{
			sound.RandomizeSFX(rushSound);  // Sound
			hero.anim.Play(rushState);      // Animation
			effect.SetRotation(dir);
			effect.Execute();            	// Effects
			movement.SetDirection(dir);
			movement.Execute();
			rushHitBoxOn = true;
			if (lockInput)
				player.inputDisabled.Add(duration);

			yield return new WaitForSeconds(duration);
			
			if (!persistAnimation)
				player.animPlayer.ResetToDefault();	// Animation
			hitEnemies.Clear();             	// Reset hit list
			rushHitBoxOn = false;
		}


		private void HandleCollideWithEnemy(Collider2D col)
		{
			if (rushHitBoxOn && col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy>();
				if (!hitEnemies.Contains(e) && hitEnemies.Count < maxHit)
				{
					hitEnemies.Add(e);
					if (OnHitEnemy != null)
						OnHitEnemy(e);
				}
			}
		}
	}
}
