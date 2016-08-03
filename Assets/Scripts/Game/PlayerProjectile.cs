﻿using UnityEngine;
using System.Collections;

// WARNING: May contain some bugs regarding hitting enemies and maps and init
public class PlayerProjectile : Projectile
{
	private Player player;
	private SimpleAnimationPlayer anim;

	[Header("Audio")]
	public AudioClip onHitSound;

	void Awake()
	{
		anim = GetComponent<SimpleAnimationPlayer> ();
		rb2d = this.GetComponent<Rigidbody2D> ();
		sr = this.GetComponent<SpriteRenderer> ();
		box = this.GetComponent<BoxCollider2D> ();
	}

	public void Init(Vector3 pos, Vector2 dir, Sprite sprite, string target, Player player, float speed = 4, int damage = 1)
	{
		base.Init (pos, dir, sprite, target, speed, damage);
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
//			Debug.Log (col.gameObject);
			Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 1.5f);
			foreach (Collider2D colChild in cols)
			{
				if (colChild.CompareTag(target))
				{
					IDamageable damageableTarget = colChild.GetComponentInChildren<IDamageable> ();
					damageableTarget.Damage (damage);
					player.TriggerOnEnemyDamagedEvent (damage);
				}
			}
			gameObject.SetActive (false);
			OnCollide ();
		}
	}
}

