//using UnityEngine;
//using System.Collections;

//public class EndPortal : Enemy
//{
//	public AudioClip damageSound;
//	public SpriteRenderer padlock;

//	public override void Init(Vector3 spawnLocation, Map map, int level)
//	{
//		// init default values
//		DEFAULT_LAYER = body.gameObject.layer;
//		DEFAULT_SPEED = body.moveSpeed;

//		this.level = level;
//		this.map = map;
//		maxHealth = baseHealth * Mathf.RoundToInt(Pawn.DamageEquation(level));  // calculate health based on level
//		health = maxHealth;
//		deathPropPool = ObjectPooler.GetObjectPooler("DeathProp");          // instantiate set object pooler

//		StartCoroutine(AnimateIn(map.CenterPosition));
//	}

//	protected override IEnumerator MoveState()
//	{
//		yield return null;
//	}

//	public override void Damage(int amt)
//	{
//		health -= amt;
//		if (health > 0)
//		{
//			sr.color = Color.red;
//			Invoke("ResetColor", 0.2f);
//			SoundManager.instance.RandomizeSFX(damageSound);
//			float healthPercentage = (float)health / maxHealth;
//		}
//		else
//		{
//			Die();
//		}
//	}

//	public override void Die()
//	{
//		RemoveEnemyFromList();
//		invincible = true;
//		StartCoroutine(DieRoutine());
//	}

//	public void Unlock()
//	{
//		RemoveEnemyFromList();
//		invincible = true;
//		anim.CrossFade("Unlock", 0f);
//		anim.SetBool("Locked", false);
//	}

//	public void Lock()
//	{
//		anim.CrossFade("Lock", 0f);
//		anim.SetBool("Locked", true);
//	}

//	protected override void SpawnDeathProps()
//	{
//		foreach (Sprite sprite in deathProps)
//		{
//			GameObject o = deathPropPool.GetPooledObject();
//			Rigidbody2D rb2d = o.GetComponent<Rigidbody2D>();
//			o.GetComponent<TempObject>().Init(
//				Quaternion.Euler(new Vector3(0, 0, 360f)),
//				this.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
//				sprite);
//			rb2d.AddTorque(Random.Range(-50f, 50f));
//			rb2d.AddForce(new Vector2(
//				Random.value - 0.5f,
//				Random.value - 0.5f),
//				ForceMode2D.Impulse);
//		}
//	}

//	protected virtual IEnumerator DieRoutine()
//	{
//		anim.CrossFade("Die", 0f);
//		yield return new WaitForSeconds(1.5f);
//		Destroy(transform.parent.gameObject);
//		SpawnDeathProps();
//	}
//}
