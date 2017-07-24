using UnityEngine;
using System.Collections;

namespace EnemyActions
{
	public class CameraShakeAction : EnemyAction
	{
		private CameraControl cam;
		public float time, magnitude;
		public bool horizontal, vertical = true;

		public override void Init(Enemy e, OnActionStateChanged onActionFinished)
		{
			base.Init(e, null);
			cam = CameraControl.instance;
		}

		public override void Execute()
		{
			base.Execute();
			cam.StartShake(time, magnitude, vertical, horizontal);
		}

		public override void Interrupt()
		{
			cam.StartShake(0, 0, false, false);
		}
	}
}

