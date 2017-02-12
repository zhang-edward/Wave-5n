using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateHitZoneAction : EnemyAction {

	public EnemyHitZone hitZone;
	public float delay;

	public override void Interrupt()
	{
		if (!interruptable)
			return;
	}

	public override void Execute()
	{
		base.Execute();
		hitZone.Activate();
	}
}
