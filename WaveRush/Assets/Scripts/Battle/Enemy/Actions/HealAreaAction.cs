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

		private ObjectPooler effectPool;

		private void OnDrawGizmosSelected()
		{
			Gizmos.DrawWireSphere(transform.position, radius);
		}

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, onActionFinished);
			effectPool = ObjectPooler.GetObjectPooler("Effect");
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
					if (enemy != this.e)
					{
						//Debug.Log("Healing");
						enemy.Heal(Mathf.RoundToInt(HealAmt()));
						numHealed++;
						PlayEffect(enemy.transform.position);
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

		private void PlayEffect(Vector3 position)
		{
			GameObject o = effectPool.GetPooledObject();
			SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
			TempObject tempObj = o.GetComponent<TempObject>();
			tempObj.info = new TempObjectInfo(true, 0f, healEffect.TimeLength, 0, new Color(1, 1, 1, 0.8f));
			anim.anim = healEffect;
			tempObj.Init(Quaternion.identity,
						 position,
			             healEffect.frames[0]);
			anim.Play();
		}
	}
}