namespace PlayerActions
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	[System.Serializable]
	public class PA_CircleCast : PlayerAction
	{
		private SoundManager sound;
		[Header("Properties")]
		public float	radius;							// The range of the attack
		public int		maxHit;                          // The maxmimum number of enemies that can be affected
		public bool		loseMomentum;					// Whether or not this ability causes the player to stand still
		public bool		disableInput = true;
		public Vector2  startPos { protected get; set; }	// The position of the area effect
		public Vector2  dir { protected get; set; }
		public float	distance { protected get; set; }
		[Header("Effects and SFX")]
		public AudioClip circleCastSound;
		public string    circleCastState = "Default";

		protected List<Enemy> hitEnemies = new List<Enemy>(); // Enemies affected with during one execution of this ability

		public delegate void HitEnemy(Enemy e);
		protected HitEnemy OnHitEnemy;


		public virtual void Init(Player player, HitEnemy onHitEnemyCallback)
		{
			base.Init(player);
			OnHitEnemy = onHitEnemyCallback;
			sound = SoundManager.instance;
		}

		public void SetCast(Vector2 start, Vector2 dir, float distance)
		{
			this.startPos = start;
			this.dir = dir.normalized;
			this.distance = distance;
		}

		protected override void DoAction()
		{
			// Sound
			sound.RandomizeSFX(circleCastSound);
			// Animation
			hero.anim.Play(circleCastState);
			// Player properties
			if (disableInput)
				player.inputDisabled.Add(duration);

			if (loseMomentum)
				hero.body.Move(Vector2.zero);

			GetEnemiesHit();
			Debug.DrawRay(startPos, dir * distance, new Color(1, 1, 1), 5.0f);
			int numEnemiesHit = 0;
			if (OnHitEnemy != null) {
				foreach(Enemy e in hitEnemies) {
					Debug.Log (e.transform.parent);
					if (numEnemiesHit < maxHit) {
						OnHitEnemy(e);
						numEnemiesHit ++;
					}
				}
			}
			hitEnemies.Clear();
		}

		protected virtual void GetEnemiesHit() {
			RaycastHit2D[] hits = Physics2D.CircleCastAll(startPos, radius, dir, distance);
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.CompareTag("Enemy"))
				{
					Enemy e = hit.collider.gameObject.GetComponentInChildren<Enemy>();
					hitEnemies.Add(e);
				}
			}
		}
	}
}
