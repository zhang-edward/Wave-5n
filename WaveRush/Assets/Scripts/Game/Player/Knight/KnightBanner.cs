using UnityEngine;

public class KnightBanner : HeroPowerUp
{
	private KnightHero knight;
	private float bannerRadius = 2.6f;
	private float bannerDuration = 10f;
	private float activateChance = 0.2f;
	private bool isBannerPresent = false;

	public GameObject bannerPrefab;

	public override void Activate(PlayerHero hero)
	{
		base.Activate(hero);
		knight = (KnightHero)hero;
		knight.player.OnEnemyLastHit += TrySpawnBanner;
	}

	public override void Stack()
	{
		base.Stack();
		activateChance += 0.1f;
	}

	public override void Deactivate()
	{
		base.Deactivate();
		knight.player.OnEnemyLastHit -= TrySpawnBanner;
	}

	private void TrySpawnBanner(Enemy e)
	{
		if (Random.value < activateChance && !isBannerPresent)
		{
			if (!e.gameObject.activeInHierarchy)
				SpawnBanner(e);
		}
			
	}

	private void SpawnBanner(Enemy e)
	{
		GameObject o = Instantiate(bannerPrefab);
		o.transform.position = e.transform.position;
		KnightBannerObject banner = o.GetComponent<KnightBannerObject>();
		banner.radius = bannerRadius;
		banner.duration = bannerDuration;
		isBannerPresent = true;
		Invoke("BannerIsGone", bannerDuration);
	}

	private void BannerIsGone()
	{
		isBannerPresent = false;
	}
}