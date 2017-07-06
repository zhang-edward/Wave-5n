using UnityEngine;

namespace EnemyActions
{
	public class PlayerInRangeCondition : EnemyCondition
	{
		public float min = 0;
		public float range;

		public override bool Check()
		{
			float dist = Vector3.Distance(transform.position, player.position);
			return dist < range && dist > min;
		}


		void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawWireSphere(transform.position, min);

			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position, range);
		}
	}
}

