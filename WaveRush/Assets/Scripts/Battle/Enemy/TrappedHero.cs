using UnityEngine;
using System.Collections;

public class TrappedHero : Enemy
{
	[Header("TrappedHero-Specific")]
	public SimpleAnimation deathEffect;
	public SimpleAnimation trappedHeroPickupEffect;
	private ObjectPooler effectPool;

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
		effectPool = ObjectPooler.GetObjectPooler("Effect");


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

	private IEnumerator DieRoutine()
	{
		anim.CrossFade("Die", 0f);
		yield return new WaitForSeconds(0.5f);
		SpawnDeathProps();
		Destroy(transform.parent.gameObject, 1.0f);
		PlayEffect(deathEffect, transform.position, 0.2f);
		yield return new WaitForSeconds(0.2f);
		CameraControl.instance.StartShake(0.2f, 0.05f);
		yield return new WaitForSeconds(0.5f);
		PlayEffect(trappedHeroPickupEffect, transform.position, 0.5f);
		transform.parent.gameObject.SetActive(false);
	}

	private void PlayEffect(SimpleAnimation toPlay, Vector3 position, float fadeOutTime)
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer animPlayer = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, toPlay.TimeLength - fadeOutTime, fadeOutTime, new Color(1, 1, 1, 0.8f));
		animPlayer.anim = toPlay;
		tempObj.Init(Quaternion.identity,
					 position,
					 toPlay.frames[0]);
		animPlayer.Play();
	}
}
