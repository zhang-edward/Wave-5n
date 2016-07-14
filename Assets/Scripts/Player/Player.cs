using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IDamageable
{
	public delegate void EnemyDamaged (float strength);
	public event EnemyDamaged OnEnemyDamaged;

	public delegate void PlayerDamaged (int damage);
	public event PlayerDamaged OnPlayerDamaged;

	public float DEFAULT_SPEED;

	public SpriteRenderer sr;
	public PlayerInput input;
	public EntityPhysics body;
	public Animator anim;

	public PlayerAbility ability;

	private int health = 10;
	private int damage = 1;

	public bool killBox = false;
	public bool isInvincible = false;

	private float primaryAbilityCooldown;
	public float cooldownTime;

	public float damagedCooldownTime = 1.0f;

	public ObjectPooler effectPool;
	public Sprite hitEffect;

	void Start()
	{
		effectPool = ObjectPooler.GetObjectPooler ("Effect");
		DEFAULT_SPEED = body.moveSpeed;
		ability.Init (this, body, anim);
	}

	public void Damage(int amt)
	{
		if (isInvincible)
			return;
		
		body.AddRandomImpulse ();
		StartCoroutine (FlashRed ());

		health -= amt;
		// TODO: check if player is dead
		OnPlayerDamaged(damage);
	}

	public void Heal(int amt)
	{
		health += amt;
	}

	/*public void PrimaryAbility()
	{
		// if cooldown has not finished
		if (primaryAbilityCooldown > 0)
			return;
		
		primaryAbilityCooldown = cooldownTime;
		killBox = true;
		body.moveSpeed = 10;
		input.isInputEnabled = false;
		anim.SetBool ("Attacking", true);
		Invoke ("ResetPrimaryAbility", 0.3f);
	}

	private void ResetPrimaryAbility()
	{
		killBox = false;
		body.moveSpeed = DEFAULT_SPEED;
		input.isInputEnabled = true;
		anim.SetBool ("Attacking", false);
	}*/

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.CompareTag("Enemy"))
		{
			if (killBox)
			{
				Enemy e = col.gameObject.GetComponentInChildren<Enemy> ();
				if (!e.hitDisabled && e.health > 0)
				{
					e.Damage (damage);
					/*Instantiate (hitEffect, 
						Vector3.Lerp (transform.position, e.transform.position, 0.5f), 
						Quaternion.identity);*/
					effectPool.GetPooledObject().GetComponent<Effect>().Init(
						Quaternion.Euler(new Vector3(0, 0, Random.Range(0, 360f))),
						Vector3.Lerp (transform.position, e.transform.position, 0.5f), 
						hitEffect,
						true,
						0);
					
					OnEnemyDamaged (damage);
				}
			}
		}
	}

	public IEnumerator FlashRed()
	{
		isInvincible = true;
		sr.color = Color.red;
		float t = 0;
		while (sr.color != Color.white)
		{
			sr.color = Color.Lerp (Color.red, Color.white, t);
			t += Time.deltaTime;
			yield return null;
		}
		isInvincible = false;
	}
}

