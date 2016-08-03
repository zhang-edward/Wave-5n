using UnityEngine;
using System.Collections;

public class FollowerEnemy : Enemy {

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

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Player"))
		{
			Player player = col.GetComponentInChildren<Player>();
			if (health > 0 && !hitDisabled)
				player.Damage (1);
		}
	}
}
