using UnityEngine;

namespace Projectiles
{
	public class EffectAction : ProjectileAction
	{
		public bool randomAngle = true;
		public SimpleAnimation anim;
		public TempObjectInfo effectInfo;

		void Start()
		{
			
		}

		public override void Execute()
		{
			Quaternion angle = Quaternion.identity;
			if (randomAngle)
				angle = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));

			EffectPooler.PlayEffect(anim, transform.position, effectInfo);
		}
	}
}
