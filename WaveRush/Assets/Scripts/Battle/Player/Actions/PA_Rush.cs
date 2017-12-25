﻿﻿namespace PlayerActions
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
		[Space]
		public int maxHit;				// the maxmimum number of enemies that can be collided with during the rush
		[Header("Effects and SFX")]
		public string     rushState = "Default";
		public AudioClip  rushSound;
		public TempObject effectObject;

		private List<Enemy> hitEnemies = new List<Enemy>();	// enemies collided with during one execution of this ability
		private bool rushHitBoxOn;

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

		protected override void DoAction()
		{
			player.StartCoroutine(RushRoutine());
		}

		private IEnumerator RushRoutine()
		{
			sound.RandomizeSFX(rushSound);  // Sound
			hero.anim.Play(rushState);      // Animation
			effect.Execute();            	// Effects

			movement.Execute();
			rushHitBoxOn = true;
			if (lockInput)
				player.input.isInputEnabled = false;

			yield return new WaitForSeconds(duration);

			player.animPlayer.ResetToDefault();	// Animation
			hitEnemies.Clear();             	// Reset hit list
			rushHitBoxOn = false;
			if (lockInput)
				player.input.isInputEnabled = true;
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
