using UnityEngine;
using System.Collections;

public class MageHero : PlayerHero {

	[Header("Class-Specific")]
	public ObjectPooler projectilePool;
	public Sprite projectileSprite;
	public Map map;

	[Header("Audio")]
	public AudioClip shootSound;
	public AudioClip teleportOutSound;
	public AudioClip teleportInSound;

	public override void Init(Player player, EntityPhysics body, Animator anim)
	{
		abilityCooldowns = new float[2];
		base.Init (player, body, anim);
	}

	public override void HandleSwipe (Vector2 dir)
	{
		base.HandleSwipe (dir);
		ShootFireball (dir);
	}

	public override void HandleTapRelease()
	{
		StartTeleport ();
	}

	private void ShootFireball(Vector2 dir)
	{
		// if cooldown has not finished
		if (abilityCooldowns[0] > 0)
			return;
		ResetCooldown (0);

		SoundManager.instance.RandomizeSFX (shootSound);
		GameObject o = projectilePool.GetPooledObject ();
		PlayerProjectile p = o.GetComponent<PlayerProjectile> ();
		body.Move (dir);
		body.Rb2d.velocity = -dir * 3f;

		p.Init (transform.position, dir, projectileSprite, "Enemy", player, map, 5f, damage);
		anim.SetTrigger ("Attack");
		Invoke ("ResetAbility", 0.5f);
	}

	private void StartTeleport()
	{
		if (map.WithinOpenCells(player.transform.position + (Vector3)player.dir) &&
			abilityCooldowns[1] <= 0)
			StartCoroutine (Teleport ());
	}

	public override void HandleHoldDown ()
	{
	}

	public void ResetAbility()
	{
		//dirIndicator.gameObject.SetActive (false);
		body.moveSpeed = player.DEFAULT_SPEED;
		//anim.ResetTrigger ("Charge");
		anim.SetTrigger ("Move");
	}

	private IEnumerator Teleport()
	{
		ResetCooldown (1);
		anim.SetTrigger ("TeleOut");
		SoundManager.instance.RandomizeSFX (teleportOutSound);
		player.isInvincible = true;
		player.input.isInputEnabled = false;
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("TeleportOut"))
			yield return null;
		player.transform.parent.position = (Vector3)player.dir + player.transform.parent.position;

		SoundManager.instance.RandomizeSFX (teleportInSound);
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("TeleportIn"))
			yield return null;
		player.isInvincible = false;
		player.input.isInputEnabled = true;
	}
}
