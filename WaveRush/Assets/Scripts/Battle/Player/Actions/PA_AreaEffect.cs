namespace PlayerActions
{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;

	[System.Serializable]
	public class PA_AreaEffect : PlayerAction
	{
		private SoundManager sound;
		[Header("Properties")]
		public float	range;							// The range of the attack
		public int		maxHit;                          // The maxmimum number of enemies that can be affected
		public bool		loseMomentum;					// Whether or not this ability causes the player to stand still
		public bool		disableInput = true;
		public Vector2 position { protected get; set; }	// The position of the area effect
		[Header("Effects and SFX")]
		public AudioClip areaAttackSound;
		public string    areaAttackState = "Default";

		protected List<Enemy> hitEnemies = new List<Enemy>(); // Enemies affected with during one execution of this ability

		public delegate void HitEnemy(Enemy e);
		protected HitEnemy OnHitEnemy;


		public virtual void Init(Player player, HitEnemy onHitEnemyCallback)
		{
			base.Init(player);
			OnHitEnemy = onHitEnemyCallback;
			sound = SoundManager.instance;
		}

		public void SetPosition(Vector2 position)
		{
			this.position = position;
		}

		protected override void DoAction()
		{
			// Sound
			sound.RandomizeSFX(areaAttackSound);
			// Animation
			hero.anim.Play(areaAttackState);
			// Player properties
			if (disableInput)
				player.inputDisabled.Add(duration);

			if (loseMomentum)
				hero.body.Move(Vector2.zero);

			GetEnemiesHit();
			int numEnemiesHit = 0;
			if (OnHitEnemy != null) {
				foreach(Enemy e in hitEnemies) {
					if (numEnemiesHit < maxHit) {
						OnHitEnemy(e);
						numEnemiesHit ++;
					}
				}
			}
			hitEnemies.Clear();
		}

		protected virtual void GetEnemiesHit() {
			Collider2D[] cols = Physics2D.OverlapCircleAll(position, range);
			foreach (Collider2D col in cols)
			{
				if (col.CompareTag("Enemy"))
				{
					Enemy e = col.gameObject.GetComponentInChildren<Enemy>();
					hitEnemies.Add(e);
				}
			}
		}
	}
}
