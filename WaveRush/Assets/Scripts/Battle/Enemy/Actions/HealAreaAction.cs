using UnityEngine;
using System.Collections;


namespace EnemyActions
{
	public class HealAreaAction : PrepareActionWrapper
	{
		[Header("Properties")]
		public float baseHealAmt;
		public float radius;
		public int maxNumHealed = 2;
		[Header("Animation")]
		public SimpleAnimation healEffect;

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, radius);
		}

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			e.healable = false;					// healers cannot heal other healers
		}

		protected override void Action()
		{
			base.Action();
			int numHealed = 0;
			Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, radius);
			foreach (Collider2D col in cols)
			{
				if (col.CompareTag("Enemy"))
				{
					Enemy enemy = col.GetComponentInChildren<Enemy>();
					if (enemy != this.e && enemy.healable)
					{
						//Debug.Log("Healing");
						enemy.Heal(Mathf.RoundToInt(HealAmt()));
						numHealed++;
						EffectPooler.PlayEffect(healEffect, enemy.transform.position);
						if (numHealed >= maxNumHealed)
							break;
					}
				}
			}
		}

		protected override void Reset()
		{
		}

		private float HealAmt()
		{
			return baseHealAmt * Pawn.DamageEquation(e.level);
		}
	}
}