﻿namespace PlayerAbilities
{
	using UnityEngine;
	using System.Collections.Generic;

	public class RushAbility : PlayerAbility
	{
		private AudioSource sound;
		[Header("Properties")]
		public float speed;					// base speed of the rush
		public float duration;				// the duration of the rush
		public int maxHit;					// the maxmimum number of enemies that can be collided with during the rush
		public bool collisionDetection;		// whether collision detection should be on
		[Header("Effects and SFX")]
		public string rushState = "Default";
		public AudioClip rushSound;
		public TempObject effectObject;
		private List<Enemy> hitEnemies = new List<Enemy>();	// enemies collided with during one execution of this ability
		private bool rushHitBoxOn;

		public delegate void HitEnemy(Enemy e);
		private HitEnemy OnHitEnemy;

		void Awake()
		{
			sound = GetComponent<AudioSource>();
		}

		public void Init(Player player, HitEnemy onHitEnemyCallback)
		{
			base.Init(player);
			OnHitEnemy = onHitEnemyCallback;
		}

		public void Execute()
		{
			// Sound
			RandomizeSFX();
			// Animation
			hero.anim.Play(rushState);
			// Effects
			PlayRushEffect();
			// Player properties
			if (collisionDetection)
				rushHitBoxOn = true;
			hero.body.moveSpeed = speed;
			hero.body.Move(player.dir.normalized);
			// reset ability
			Invoke("Reset", duration);
		}

		public void Reset()
		{
			// Animation
			hero.anim.Play("Default");
			// Player Properties
			hitEnemies.Clear(); // Reset hit list
			rushHitBoxOn = false;
			hero.body.moveSpeed = player.DEFAULT_SPEED;
		}

		private void PlayRushEffect()
		{
			SimpleAnimationPlayer animPlayer = effectObject.GetComponent<SimpleAnimationPlayer>();

			Vector2 dir = player.dir;
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
			TempObjectInfo info = new TempObjectInfo();
			info.targetColor = new Color(1, 1, 1, 0.5f);
			info.lifeTime = duration;
			info.fadeOutTime = 0.1f;
			effectObject.Init(
				Quaternion.Euler(new Vector3(0, 0, angle)),
				transform.position,
				animPlayer.anim.frames[0],
				info);
			animPlayer.Play();
		}

		void OnTriggerStay2D(Collider2D col)
		{
			if (rushHitBoxOn)
			{
				if (col.CompareTag("Enemy"))
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

		public void RandomizeSFX()
		{
			float randomPitch = Random.Range(0.95f, 1.05f);
			sound.pitch = randomPitch;
			sound.clip = rushSound;
			sound.Play();
		}
	}
}
