using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class ESA_CreateEffectAction : EnemyAction
	{
		public Transform location;
		public TempObjectInfo info;
		public bool randomizeRotation = false;
		public SimpleAnimation anim;

		public override void Execute()
		{
			base.Execute();
			Quaternion rot;
			if (randomizeRotation)
				rot = Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360)));
			else 
				rot = Quaternion.identity;
			EffectPooler.PlayEffect(anim, location.position, info, rot);
			print ("Done playing effect");
			if (onActionFinished != null)
				onActionFinished();
		}

		public override void Interrupt()
		{
		}
	}
}

