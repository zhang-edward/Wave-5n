using UnityEngine;
using System.Collections;

public class FollowerEnemy : Enemy {

	// Update is called once per frame
	void Update () {
		if (isPlayerNearby ())
		{
		}
		else
		{
			
		}

	}

	protected override IEnumerator MoveState()
	{
		while (true)
		{
			body.move ((player.position - transform.position).normalized);
			yield return null;
		}
	}
}
