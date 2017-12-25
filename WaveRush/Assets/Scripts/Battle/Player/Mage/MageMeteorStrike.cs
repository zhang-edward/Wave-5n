using UnityEngine;
using Projectiles;

public class MageMeteorStrike : HeroPowerUp
{
	public GameObject meteorPrefab;
	public AudioClip spawnMeteorSound;

	private RuntimeObjectPooler projectilePool;
	private MageFireOrbs fireOrbs;
	private MageHero mage;

	private float timer = 0f;
	private const float HOLD_DOWN_TIME = 0.5f;
	private bool droppedMeteor;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		mage = (MageHero)hero;
		fireOrbs = mage.GetComponentInChildren<MageFireOrbs>();
		mage.onTapHoldDown += DropMeteor;
		mage.onTapRelease += ResetTap;
		projectilePool = (RuntimeObjectPooler)meteorPrefab.GetComponent<Projectile>().GetObjectPooler();
	}

	private void DropMeteor()
	{
		timer += Time.deltaTime;
		if (timer < HOLD_DOWN_TIME || droppedMeteor || fireOrbs.orbsActive <= 0)
			return;

		SoundManager.instance.RandomizeSFX(spawnMeteorSound);
		fireOrbs.DeactivateOrb();

		Vector3 target = (Vector3)playerHero.player.dir + transform.position;
		GameObject meteorObj = projectilePool.GetPooledObject();

		meteorObj.GetComponentInChildren<AreaDamageAction>().damage = Mathf.RoundToInt(mage.damage * 4f);
		Vector2 spawnPos = (Vector2)target + new Vector2(Random.Range(-3f, 3f), 10);
		Vector2 dir = (Vector2)target - spawnPos;
		Projectile meteor = meteorObj.GetComponent<Projectile>();
		meteor.Init(spawnPos, dir);
		droppedMeteor = true;
	}

	private void ResetTap()
	{
		timer = 0;
		droppedMeteor = false;
	}
}
