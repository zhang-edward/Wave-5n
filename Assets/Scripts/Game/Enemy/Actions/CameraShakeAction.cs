using UnityEngine;
using System.Collections;

public class CameraShakeAction : EnemyAction
{
	private CameraControl cam;
	public float delay;
	public float time, magnitude;

	public override void Init (Enemy e, OnActionStateChanged onActionFinished)
	{
		base.Init (e, onActionFinished);
		cam = CameraControl.instance;
	}

	public override void Execute ()
	{
		cam.StartShake (time, magnitude);
	}

	public override void Interrupt ()
	{
		base.Interrupt ();
		cam.StartShake (0, 0);
	}
}

