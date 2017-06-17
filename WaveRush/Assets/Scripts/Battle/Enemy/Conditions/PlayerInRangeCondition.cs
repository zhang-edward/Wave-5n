using UnityEngine;

namespace EnemyActions
{
	public class PlayerInRangeCondition : EnemyCondition
	{
		public float range;

		public override bool Check()
		{
			Collider2D[] cols = Physics2D.OverlapCircleAll(e.transform.position, range);
			foreach (Collider2D col in cols)
			{
				if (col.CompareTag("Player"))
				{
					return true;
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

