namespace EnemyActions
{
	using UnityEngine;
	using System.Collections;

	public class EA_Lunge : PrepareActionWrapper
	{
		private EntityPhysics body;
		private float defaultSpeed;
		private bool attacking = false;
		private Vector2 dir;

		[Header("Properties")]
		public float lungeSpeed = 10.0f;
		public int baseDamage;


		public override void Init(Enemy e, OnActionStateChanged onActionFinished) {
			base.Init(e, onActionFinished);
			body = e.body;
			defaultSpeed = e.DEFAULT_SPEED;
		}

		public override void Interrupt()
		{
			base.Interrupt();
			body.moveSpeed = defaultSpeed;
			body.Move(Vector3.zero);
			attacking = false;
		}

		protected override void Charge()
		{
			base.Charge();
			//anim.ResetTrigger ("Charge");
			body.Move(Vector2.zero);
			dir = (Vector2)(e.playerTransform.position - transform.position); // freeze moving direction
		}

		protected override void Action()
		{
			base.Action();
			attacking = true;

			body.moveSpeed = lungeSpeed;
			body.Move(dir.normalized);
		}

		protected override void Reset()
		{
			attacking = false;
			body.moveSpeed = defaultSpeed;
			body.Move(dir.normalized);
		}

		protected void OnTriggerStay2D(Collider2D col)
		{
			if (col.CompareTag("Player"))
			{
				if (attacking)
				{
					IDamageable player = col.GetComponentInChildren<IDamageable>();
					int damage = Formulas.EnemyDamage(baseDamage, e.GetLevelDiff());
					player.Damage(damage, e);
				}
			}
		}
	}
}
