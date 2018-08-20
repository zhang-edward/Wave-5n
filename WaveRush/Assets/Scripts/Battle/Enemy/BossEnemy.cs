using UnityEngine;
using System.Collections;

public class BossEnemy : Enemy {
	public const string POOL_SOULS = "Souls";
	
	protected EnemyManager enemyManager;
	[Header("Boss Attributes")]
	public AudioClip deathSound;
	public bool dying { get; private set; }

	private SimpleAnimation deathEffect;
	private int numSouls;

	public override void Init (Vector3 spawnLocation, Map map, int level)
	{
		canBeDisabled = false;
		numSouls = Formulas.BossSoulsDrop(level);
		base.Init (spawnLocation, map, level);
		enemyManager = GetComponentInParent<EnemyManager> ();
		deathEffect = enemyManager.bossDeathEffect;
		CameraControl.instance.secondaryFocus = this.transform;
	}

	public override void Die ()
	{
		player.StartTempSlowDown(1.0f);
		// print ("Boss dying");
		action.Interrupt();
		foreach (EnemyStatus status in statuses)
		{
			status.gameObject.SetActive (false);
		}
		foreach (Enemy e in enemyManager.Enemies)
		{
			if (e.isActiveAndEnabled && e as BossEnemy == null)
				e.ForceDisable (2f);
		}
		StopAllCoroutines ();
		StartCoroutine (DeathAnimation());
	}

	protected override IEnumerator MoveState()
	{
		if (dying || health <= 0)
		{
			// print("Boss dying from moveState");
			Die();
			yield break;
		}
		yield return base.MoveState();
	}

	public override void Damage(int amt, IDamageable source)
	{
		if (dying)
			return;
		base.Damage(amt, source);
	}

	private IEnumerator DeathAnimation()
	{
		// print("Start dying");
		CameraControl cam = CameraControl.instance;
		anim.Play ("Dead");
		while (OtherBossesDying())
			yield return null;
		dying = true;
		cam.SetFocus (this.transform);
		yield return new WaitForSeconds(1f);
		EffectPooler.PlayEffect(deathEffect, transform.position);
		float delay = deathEffect.SecondsPerFrame * 3f;
		yield return new WaitForSeconds(delay);
		SoundManager.instance.RandomizeSFX (deathSound);
		cam.StartShake (0.5f, 0.05f, false, true);
		cam.StartFlashColor (Color.white, 1, 0, 0, 1);
		dying = false;
		cam.secondaryFocus = null;
		cam.ResetFocus ();
		SpawnSouls();
		base.Die ();

		yield return null;
	}

	private void SpawnSouls()
	{
		for (int i = 0; i < numSouls; i++)
		{
			GameObject o = ObjectPooler.GetObjectPooler(POOL_SOULS).GetPooledObject();
			o.SetActive(true);
			o.transform.position = transform.position;
			o.GetComponent<SoulPickup>().Init(playerTransform);
		}
	}

	private bool OtherBossesDying()
	{
		foreach (BossEnemy boss in enemyManager.bosses) {
			if (boss.dying && boss != this) {
				return true;
			}
		}
		return false;
	}
}

