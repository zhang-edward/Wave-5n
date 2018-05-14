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
		public float   range;							// The range of the attack
		public int 	   maxHit;                          // The maxmimum number of enemies that can be affected
		public bool	   loseMomentum;					// Whether or not this ability causes the player to stand still
		public Vector2 position { private get; set; }	// The position of the area effect
		[Header("Effects and SFX")]
		public AudioClip areaAttackSound;
		public string    areaAttackState = "Default";

		private List<Enemy> hitEnemies = new List<Enemy>(); // Enemies affected with during one execution of this ability

		public delegate void HitEnemy(Enemy e);
		private HitEnemy OnHitEnemy;


		public void Init(Player player, HitEnemy onHitEnemyCallback)
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
			player.StartCoroutine(ExecuteRoutine(position));
		}

		private IEnumerator ExecuteRoutine(Vector2 pos)
		{
			// Sound
			sound.RandomizeSFX(areaAttackSound);
			// Animation
			hero.anim.Play(areaAttackState);
			// Player properties
			player.input.isInputEnabled = false;

			if (loseMomentum)
				hero.body.Move(Vector2.zero);

			int numEnemiesHit = 0;
			Collider2D[] cols = Physics2D.OverlapCircleAll(pos, range);
			foreach (Collider2D col in cols)
			{
				if (col.CompareTag("Enemy"))
				{
					numEnemiesHit++;
					if (numEnemiesHit < maxHit)
					{
						Enemy e = col.gameObject.GetComponentInChildren<Enemy>();
						hitEnemies.Add(e);
						if (OnHitEnemy != null)
							OnHitEnemy(e);
					}
				}
			}
			//Debug.Log("Detected " + numEnemiesHit + " enemies");

			yield return new WaitForSeconds(duration);
			player.input.isInputEnabled = true;
		}
	}
}
