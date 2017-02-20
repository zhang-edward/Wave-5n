using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class CameraShakeAction : EnemyAction
	{
		private CameraControl cam;
		public float time, magnitude;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, null);
			cam = CameraControl.instance;
		}

		public override void Execute()
		{
			base.Execute();
			cam.StartShake(time, magnitude);
		}

		public override void Interrupt()
		{
			if (!interruptable)
				return;
			cam.StartShake(0, 0);
		}
	}
}

