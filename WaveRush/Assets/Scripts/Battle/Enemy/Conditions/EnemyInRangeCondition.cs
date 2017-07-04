using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class EnemyInRangeCondition : EnemyCondition
	{
		public float range;

		public override bool Check()
		{
			Collider2D[] cols = Physics2D.OverlapCircleAll(e.transform.position, range);
			foreach (Collider2D col in cols)
			{
				if (col.CompareTag("Enemy"))
				{
					Enemy enemy = col.GetComponentInChildren<Enemy>();
					if (enemy != this.e)
					{
						return true;
					}
				}
			}
			return false;
		}


		void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, range);
		}
	}
}
