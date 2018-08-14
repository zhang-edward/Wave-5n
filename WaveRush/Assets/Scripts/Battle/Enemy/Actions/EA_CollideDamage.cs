namespace EnemyActions
{
	using UnityEngine;

	public class EA_CollideDamage : EnemyAction
	{
		private float cooldown;
		private float attackBuildUp = 0.4f; // time for the player to be in contact with the enemy before the player is damaged
		private float buildUp;              // timer for attackBuildUp

		public int baseDamage = 1;
		public bool activated;
		public float attackCooldown = 1f;

		public override void Execute()
		{
			base.Execute();
			activated = true;
			onActionFinished();
		}

		public override bool CanExecute() {
			return !activated;
		}

		public override void Interrupt()
		{
		}

		void Update()
		{
			cooldown -= Time.deltaTime;
		}

		void OnTriggerStay2D(Collider2D col)
		{
			if (!activated)
				return;
			if (col.CompareTag("Player"))
			{
				Player player = col.GetComponentInChildren<Player>();
				if (cooldown <= 0 && e.health > 0 && !e.hitDisabled && buildUp >= attackBuildUp)
				{
					int damage = Formulas.EnemyDamageFormula(baseDamage, player.hero.level - e.level);
					player.Damage(damage, e);
					cooldown = attackCooldown;
				}
				else
				{
					buildUp += Time.deltaTime;
				}
			}
		}

		void OnTriggerExit2D(Collider2D col)
		{
			if (col.CompareTag("Player"))
			{
				buildUp = 0;
			}
		}
	}
}