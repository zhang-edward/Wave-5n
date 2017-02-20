using UnityEngine;

namespace Projectiles
{
	public class EffectAction : ProjectileAction
	{
		private ObjectPooler effectPool;

		public SimpleAnimation anim;
		public TempObjectInfo effectInfo;

		void Start()
		{
			effectPool = ObjectPooler.GetObjectPooler("Effect");
		}

		public override void Execute()
		{
			TempObject effect = effectPool.GetPooledObject().GetComponent<TempObject>();
			SimpleAnimationPlayer animPlayer = effect.GetComponent<SimpleAnimationPlayer>();
			animPlayer.anim = this.anim;
			effect.Init(
				Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360))),
				transform.position,
				anim.frames[0],
				effectInfo
			);
			animPlayer.Play();
		}
	}
}
