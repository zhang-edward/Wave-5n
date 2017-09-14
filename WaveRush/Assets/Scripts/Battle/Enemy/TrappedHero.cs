using UnityEngine;
using System.Collections;

public class TrappedHero : Enemy
{
	[Header("TrappedHero-Specific")]
	public SimpleAnimation deathEffect;
	public SimpleAnimation trappedHeroPickupEffect;
	public AudioClip damageSound;
	public AudioClip deathSound;
	public AudioClip acquireHeroSound;

	public override void Init(Vector3 spawnLocation, Map map, int level)
	{
		// init default values
		DEFAULT_LAYER = body.gameObject.layer;
		DEFAULT_SPEED = body.moveSpeed;

		this.level = level;
		this.map = map;
		maxHealth = baseHealth * Mathf.RoundToInt(Pawn.DamageEquation(level));  // calculate health based on level
		health = maxHealth;
		deathPropPool = ObjectPooler.GetObjectPooler("DeathProp");          // instantiate set object pooler

		StartCoroutine(AnimateIn(spawnLocation));
	}

	protected override IEnumerator MoveState()
	{
		yield return null;
	}

	public override void Damage(int amt)
	{
		health -= amt;
		if (health > 0)
		{
			sr.color = Color.red;
			Invoke("ResetColor", 0.2f);
			SoundManager.instance.RandomizeSFX(damageSound);
		}
		else
		{
			Die();
		}
	}

	public override void AddStatus(GameObject statusObj)
	{
		Destroy(statusObj.gameObject);
	}

	public override void Die()
	{
		RemoveEnemyFromList();
		invincible = true;
		Pawn pawn = PawnGenerator.GenerateCrystalDrop(level);
		if (BattleSceneManager.instance != null)
			BattleSceneManager.instance.AddPawn(pawn);
		StartCoroutine(DieRoutine());
	}

	protected override void SpawnDeathProps()
	{
		foreach (Sprite sprite in deathProps)
		{
			GameObject o = deathPropPool.GetPooledObject();
			Rigidbody2D rb2d = o.GetComponent<Rigidbody2D>();
			o.GetComponent<TempObject>().Init(
				Quaternion.Euler(new Vector3(0, 0, 360f)),
				this.transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)),
				sprite);
			rb2d.AddTorque(Random.Range(-50f, 50f));
			rb2d.AddForce(new Vector2(
				Random.value - 0.5f,
				Random.value - 0.5f),
				ForceMode2D.Impulse);
		}
	}

	protected virtual IEnumerator DieRoutine()
	{
		anim.CrossFade("Die", 0f);
		yield return new WaitForSeconds(0.5f);
		SpawnDeathProps();
		Destroy(transform.parent.gameObject, 1.0f);
		EffectPooler.PlayEffect(deathEffect, transform.position, false, 0f);
		yield return new WaitForSeconds(0.2f);
		CameraControl.instance.StartShake(0.2f, 0.05f, false, true);
		SoundManager.instance.RandomizeSFX(deathSound);
		yield return new WaitForSeconds(0.3f);
		EffectPooler.PlayEffect(trappedHeroPickupEffect, transform.position, false, 0.5f);
		transform.parent.gameObject.SetActive(false);
		SoundManager.instance.RandomizeSFX(acquireHeroSound);
	}
}
