using UnityEngine;
using System.Collections;

public class PlayerProjectile : Projectile
{
	private const int MAX_HIT = 5; 	// The maxmimum number of enemies the area attack can hit
	private Player player;
	private SimpleAnimationPlayer anim;

	// SET IN INSPECTOR!
	public float setSpeed;
	public int setDamage;
	public bool areaAttack;				// whether this should effect an area upon impact
	public float areaAttackRadius;		// the radius of the area attack, if enabled
	public bool disabledOnImpact = true;		// whether the projectile is disabled on impact

	[Header("Audio")]
	public AudioClip onHitSound;

	void Awake()
	{
		anim = GetComponent<SimpleAnimationPlayer> ();
		rb2d = this.GetComponent<Rigidbody2D> ();
		sr = this.GetComponent<SpriteRenderer> ();
		box = this.GetComponent<BoxCollider2D> ();
	}

	public void Init(Vector3 pos, Vector2 dir, Player player)
	{
		base.Init (pos, dir, anim.anim.frames[0], target, setSpeed, setDamage);
		this.player = player;
		anim.looping = true;
		anim.Play ();
	}

	public void Init(Vector3 pos, Vector2 dir, Player player, float speed, int damage)
	{
		base.Init (pos, dir, anim.anim.frames[0], target, speed, damage);
		this.player = player;
		anim.looping = true;
		anim.Play ();
	}

	void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere (transform.position, 1.5f);
	}

	protected override void OnTriggerEnter2D (Collider2D col)
	{
		//Debug.Log (col.tag);
		if (col.CompareTag(target))
		{
			SoundManager.instance.RandomizeSFX (onHitSound);
			if (areaAttack)
				AreaAttack ();
			else
			{
				IDamageable damageableTarget = col.GetComponentInChildren<IDamageable> ();
				damageableTarget.Damage (damage);
				player.TriggerOnEnemyDamagedEvent (damage);
			}
			OnCollide ();
			if (disabledOnImpact)
				gameObject.SetActive (false);
			
		}
	}

	private void AreaAttack()
	{
		int numEnemiesHit = 0;
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 1.5f);
		foreach (Collider2D colChild in cols)
		{
			if (colChild.CompareTag(target))
			{
				numEnemiesHit++;
				if (numEnemiesHit < MAX_HIT)
				{
					IDamageable damageableTarget = colChild.GetComponentInChildren<IDamageable> ();
					damageableTarget.Damage (damage);
					player.TriggerOnEnemyDamagedEvent (damage);
				}
			}
		}
	}
}

