using UnityEngine;
using System.Collections;

public class MageAbility : PlayerAbility {

	public ObjectPooler projectilePool;
	public Sprite projectileSprite;

	//public Transform dirIndicator;

	//private Vector3 shootPoint;

	public override void Init(Player player, EntityPhysics body, Animator anim)
	{
		base.Init (player, body, anim);
	}

/*	private IEnumerator SetShootPoint()
	{
		while (true)
		{
			shootPoint = new Vector3 (0.35f * Mathf.Sign (player.dir.x), 0.3f);
//			Debug.Log (shootPoint);
			yield return null;
		}
	}*/

	public override void Ability()
	{
		// if cooldown has not finished
		if (abilityCooldown > 0)
			return;

		abilityCooldown = cooldownTime;

		GameObject o = projectilePool.GetPooledObject ();
		MageProjectile p = o.GetComponent<MageProjectile> ();

		// use auto targeter
/*		Vector3 dir;
		if (player.targetedEnemy != null)
			dir = player.targetedEnemy.position - transform.position;
		else
			dir = player.dir;
		player.StopAutoTarget();*/
		Vector3 dir = player.dir;
		
		p.Init (transform.position, dir, projectileSprite, "Enemy", player, 5f, 1);
		anim.SetBool ("Charge", false);
		anim.SetTrigger ("Attack");
		Invoke ("ResetAbility", 0.5f);
	}

	public override void AbilityHoldDown ()
	{
		if (abilityCooldown > 0)
			return;
		anim.SetBool ("Charge", true);
		body.moveSpeed = 0.3f;

		//player.StartAutoTarget ();

		//dirIndicator.gameObject.SetActive (true);
		//float angle = Mathf.Atan2 (player.dir.y, player.dir.x) * Mathf.Rad2Deg;
		//Quaternion rot = Quaternion.Euler (0, 0, angle);
		//dirIndicator.rotation = rot;
		//dirIndicator.localPosition = shootPoint;
		//dirIndicator.rotation = Quaternion.Lerp (dirIndicator.rotation, rot, 0.1f);
	}

	public override void ResetAbility()
	{
		//dirIndicator.gameObject.SetActive (false);
		body.moveSpeed = player.DEFAULT_SPEED;
		//anim.ResetTrigger ("Charge");
		anim.SetTrigger ("Move");
	}
}
