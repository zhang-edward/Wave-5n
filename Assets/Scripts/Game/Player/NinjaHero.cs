using UnityEngine;
using System.Collections;

public class NinjaHero : PlayerHero {

	[Header("Class-Specific")]
	public RuntimeObjectPooler projectilePool;
	public GameObject projectilePrefab;
	public int smokeBombRange = 2;
	public float dashDistance = 4;

	public SimpleAnimation hitEffect;
	public SimpleAnimation ninjaStarAnim;

	[Header("Audio")]
	public AudioClip shootSound;
	public AudioClip slashSound;

	//public int damage = 1;

	public override void Init(Player player, EntityPhysics body, Animator anim)
	{
		abilityCooldowns = new float[2];
		base.Init (player, body, anim);
		heroName = PlayerHero.NINJA;
		projectilePool.SetPooledObject(projectilePrefab);
	}

	public override void HandleSwipe ()
	{
		if (abilityCooldowns [0] > 0)
			return;
		ResetCooldown (0);
		StartCoroutine(DashAttack ());
	}

	public override void HandleTapRelease()
	{
		//smokeBomb.Init (transform.position);
		ShootNinjaStar();
	}

	private void ShootNinjaStar()
	{
		// if cooldown has not finished
		if (abilityCooldowns[1] > 0)
			return;
		ResetCooldown (1);
		// Sound
		SoundManager.instance.RandomizeSFX (shootSound);
		// Animation
		anim.SetBool ("Attack", true);

		PlayerProjectile ninjaStar = projectilePool.GetPooledObject ().GetComponent<PlayerProjectile>();
		Vector2 dir = player.dir.normalized;
		ninjaStar.Init (transform.position, dir, player);
		// set direction
		body.Move (dir);
		body.Rb2d.velocity = Vector2.zero;

		Invoke ("ResetAbility", 0.5f);
	}

	public void ResetAbility()
	{
		body.moveSpeed = player.DEFAULT_SPEED;
		anim.SetBool("Attack", false);
	}

	private IEnumerator DashAttack()
	{
		player.isInvincible = true;
		player.input.isInputEnabled = false;

		anim.SetTrigger ("DashOut");
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("DashOut"))
			yield return null;

		//Vector3 testDestination = (Vector3)player.dir.normalized * dashDistance 
		//	+ player.transform.parent.position;
		float distance = GetDashDistanceClamped (transform.position, player.dir.normalized);
		Vector3 dest = (Vector3)player.dir.normalized * distance 
			+ player.transform.parent.position;
		
		body.Move (player.dir.normalized);		
		player.isInvincible = false;
		DashCircleCast (transform.position, dest);
		player.transform.parent.position = dest;

		AreaAttack ();
		yield return new WaitForEndOfFrame ();		// wait for the animation state to update before continuing
		while (anim.GetCurrentAnimatorStateInfo (0).IsName ("DashIn"))
			yield return null;

		player.input.isInputEnabled = true;
	}

	/// <summary>
	/// Do the circle cast attack with origin being the original player position and dest
	/// the final player position before and after the dash attack
	/// </summary>
	/// <param name="origin">Origin.</param>
	/// <param name="dest">Destination.</param>
	private void DashCircleCast(Vector3 origin, Vector3 dest)
	{
		RaycastHit2D[] hits = Physics2D.CircleCastAll (origin, 0.5f, (dest - origin), dashDistance);
		foreach (RaycastHit2D hit in hits)
		{
			if (hit.collider.CompareTag("Enemy"))
			{
				Enemy e = hit.collider.GetComponentInChildren<Enemy> ();
				DamageEnemy (e);
			}
		}
	}

	private float GetDashDistanceClamped(Vector3 start, Vector2 dir)
	{
		RaycastHit2D hit = Physics2D.Raycast (start, dir, dashDistance, 1 << LayerMask.NameToLayer("MapCollider"));
		Debug.DrawRay (start, dir * dashDistance, new Color(1, 1, 1), 5.0f);
//		Debug.Log (hit.collider.gameObject);
		if (hit.collider != null)
		{
			if (hit.collider.CompareTag ("MapBorder"))
			{
				return hit.distance - 0.5f;		// compensate for linecast starting from middle of body
			}
		}
		return dashDistance;
	}

	private void DamageEnemy(Enemy e)
	{
		if (!e.invincible && e.health > 0)
		{
			e.Damage (damage);
			TempObject o = player.effectPool.GetPooledObject ().GetComponent<TempObject> ();
			SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer> ();
			anim.anim = hitEffect;
			o.Init (
				Quaternion.Euler (new Vector3 (0, 0, Random.Range (0, 360f))),
				e.transform.position, 
				hitEffect.frames[0],
				true,
				0,
				0.2f);
			anim.Play ();
			

			player.TriggerOnEnemyDamagedEvent(damage);
		}
	}

	private void AreaAttack()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 1.5f);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag("Enemy"))
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				DamageEnemy (e);
			}
		}
	}
}
