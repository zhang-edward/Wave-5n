using UnityEngine;
using System.Collections;

public class BossEnemy : Enemy
{
	[Header("Boss Attributes")]
	public AudioClip deathSound;
	public SimpleAnimation deathEffect;
	public bool dying { get; private set; }

	protected EnemyManager enemyManager;
	private ObjectPooler effectPool;

	public override void Init (Vector3 spawnLocation, Map map, int level)
	{
		canBeDisabledOnHit = false;
		base.Init (spawnLocation, map, level);
		effectPool = ObjectPooler.GetObjectPooler("Effect");
		enemyManager = GetComponentInParent<EnemyManager> ();
		CameraControl.instance.secondaryFocus = this.transform;
	}

	public override void Die ()
	{
		print ("Boss dying");
		action.Interrupt();
		foreach (EnemyStatus status in statuses)
		{
			status.gameObject.SetActive (false);
		}
		foreach (Enemy e in enemyManager.Enemies)
		{
			if (e.isActiveAndEnabled && e as BossEnemy == null)
				e.Disable (4f);
		}
		StopAllCoroutines ();
		StartCoroutine ("DeathAnimation");
	}

	protected override IEnumerator MoveState()
	{
		if (dying)
			Die();
		return base.MoveState();
	}

	public override void Damage(int amt)
	{
		if (dying)
			return;
		base.Damage(amt);
	}

	private IEnumerator DeathAnimation()
	{
		//print("Start dying");
		CameraControl cam = CameraControl.instance;
		anim.CrossFade ("Dead", 0f);
		while (OtherBossesDying())
			yield return null;
		dying = true;
		cam.SetFocus (this.transform);
		yield return new WaitForSeconds(1f);
		PlayEffect(deathEffect, transform.position, 0f);
		float delay = deathEffect.SecondsPerFrame * 3f;
		yield return new WaitForSeconds(delay);
		SoundManager.instance.RandomizeSFX (deathSound);
		cam.StartShake (0.5f, 0.05f);
		cam.StartFlashColor (Color.white);
		dying = false;
		cam.secondaryFocus = null;
		cam.ResetFocus ();
		base.Die ();
		yield return null;
	}

	private bool OtherBossesDying()
	{
		foreach (BossEnemy boss in enemyManager.bosses)
		{
			if (boss.dying)
				return true;
		}
		return false;
	}

	private void PlayEffect(SimpleAnimation toPlay, Vector3 position, float fadeOutTime)
	{
		GameObject o = effectPool.GetPooledObject();
		SimpleAnimationPlayer anim = o.GetComponent<SimpleAnimationPlayer>();
		TempObject tempObj = o.GetComponent<TempObject>();
		tempObj.info = new TempObjectInfo(true, 0f, toPlay.TimeLength - fadeOutTime, fadeOutTime, new Color(1, 1, 1, 0.8f));
		anim.anim = toPlay;
		tempObj.Init(Quaternion.identity,
					 position,
					 toPlay.frames[0]);
		anim.Play();
	}
}

