using UnityEngine;
using System.Collections;

public abstract class BossEnemy : Enemy
{
	public AudioClip deathSound;
	protected EnemyManager enemyManager;
	public bool dying { get; private set; }

	public override void Init (Vector3 spawnLocation, Map map)
	{
		base.Init (spawnLocation, map);
		enemyManager = GetComponentInParent<EnemyManager> ();
		CameraControl.instance.secondaryFocus = this.transform;
	}

	void Update()
	{
		if (health <= 0 && !dying)
		{
			Die();
		}
	}

	public override void Die ()
	{
		if (dying)
			return;
		//print ("Boss dying");
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

	private IEnumerator DeathAnimation()
	{
		CameraControl camera = CameraControl.instance;
		//print ("dying");
		dying = true;
		camera.SetFocus (this.transform);
		anim.CrossFade ("Dead", 0f);
		yield return new WaitForSeconds (1f);
		camera.StartShake (0.5f, 0.05f);
		camera.StartFlashColor (Color.white);
		SoundManager.instance.RandomizeSFX (deathSound);
		yield return new WaitForSeconds (2f);
		camera.secondaryFocus = null;
		camera.ResetFocus ();
		base.Die ();
		dying = false;
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
}

