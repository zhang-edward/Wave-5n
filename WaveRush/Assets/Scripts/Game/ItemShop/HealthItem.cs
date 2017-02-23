using UnityEngine;
using System.Collections;

public class HealthItem : UpgradeItem
{
	public override void Upgrade(Player player)
	{
		player.HealEffect (4, true);
	}
}

