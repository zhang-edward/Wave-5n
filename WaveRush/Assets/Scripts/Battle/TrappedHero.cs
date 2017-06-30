using UnityEngine;
using System.Collections;

public class TrappedHero : Enemy
{
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
		SpawnDeathProps();
		transform.parent.gameObject.SetActive(false);
		Destroy(transform.parent.gameObject, 1.0f);

		int numHeroTypes = System.Enum.GetNames(typeof(HeroType)).Length;
		//HeroType type = (HeroType)Enum.GetValues(typeof(HeroType)).GetValue(UnityEngine.Random.Range(1, numHeroTypes));
		Pawn pawn = new Pawn();
		pawn.level = UnityEngine.Random.Range(0, 10);
		pawn.type = HeroType.Knight;//type;
		GameManager.instance.saveGame.AddPawn(pawn);
	}
}
