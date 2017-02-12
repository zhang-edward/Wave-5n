using UnityEngine;

public class PlayerInHitZoneCondition : EnemyCondition
{
	public PlayerDetectionCircle hitZone;

	public override bool Check()
	{
		return hitZone.Activate() != null;	// PlayerDetectionCircle.Activate() returns a Player in the detection area
	}
}
