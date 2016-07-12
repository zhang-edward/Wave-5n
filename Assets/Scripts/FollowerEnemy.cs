using UnityEngine;
using System.Collections;

public class FollowerEnemy : Enemy {

	// Update is called once per frame
	void Update () {
		if (PlayerInRange ())
		{
		}
		else
		{
		}

	}

	protected override IEnumerator MoveState()
	{
//		Debug.Log ("MoveState: Enter");
		while (true)
		{
			body.Move ((player.position - transform.position).normalized);
			yield return null;
		}
	}

	protected override void ResetVars ()
	{
		body.gameObject.layer = DEFAULT_LAYER;
		body.moveSpeed = DEFAULT_SPEED;
	}
}
