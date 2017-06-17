using UnityEngine;
using Projectiles;
using System.Collections;

public class MageMeteorShower : HeroPowerUp
{
	public GameObject meteorPrefab;
	public AudioClip spawnMeteorSound;

	private const int NUM_METEORS = 10;
	private RuntimeObjectPooler projectilePool;
	private MageFireOrbs fireOrbs;
	private MageHero mage;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		mage = (MageHero)hero;
		fireOrbs = mage.GetComponentInChildren<MageFireOrbs>();
		projectilePool = (RuntimeObjectPooler)meteorPrefab.GetComponent<Projectile>().GetObjectPooler();
		mage.OnMageSpecialAbility += StartMeteorShowerRoutine;
	}

	private void StartMeteorShowerRoutine()
	{
		StartCoroutine(MeteorShower());
	}

	private IEnumerator MeteorShower()
	{
		int meteorsDropped = 0;
		while (meteorsDropped < NUM_METEORS)
		{
			DropMeteor(new Vector3(Random.Range(5, Map.size - 5), Random.Range(5, Map.size - 5)));
			meteorsDropped++;
			yield return new WaitForSeconds(Random.Range(0.5f, 1f));
		}
	}

	private void DropMeteor(Vector3 target)
	{
		SoundManager.instance.RandomizeSFX(spawnMeteorSound);

		GameObject meteorObj = projectilePool.GetPooledObject();
		Vector2 spawnPos = (Vector2)target + new Vector2(Random.Range(-3f, 3f), 10);
		Vector2 dir = (Vector2)target - spawnPos;
		Projectile meteor = meteorObj.GetComponent<Projectile>();
		meteor.Init(spawnPos, dir);
	}
}
