using UnityEngine;
using System.Collections;

public abstract class BossEnemy : Enemy
{
	public AudioClip deathSound;
	protected EnemyManager enemyManager;

	public override void Init (Vector3 spawnLocation, Map map)
	{
		base.Init (spawnLocation, map);
		enemyManager = GetComponentInParent<EnemyManager> ();
		CameraControl.instance.secondaryFocus = this.transform;
	}

	public override void Die ()
	{
		foreach (Enemy e in enemyManager.Enemies)
		{
			e.Disable (4f);
		}
		StopAllCoroutines ();
		StartCoroutine ("DeathAnimation");
	}

	private IEnumerator DeathAnimation()
	{
		CameraControl camera = CameraControl.instance;

		camera.SetFocus (this.transform);
		anim.CrossFade ("Dead", 0f);
		Debug.Log ("Dying");
		yield return new WaitForSeconds (1f);
		Debug.Log ("Dead");
		camera.StartShake (0.5f, 0.05f);
		camera.StartFlashColor (Color.white);
		SoundManager.instance.RandomizeSFX (deathSound);
		yield return new WaitForSeconds (2f);
		camera.secondaryFocus = null;
		camera.ResetFocus ();
		base.Die ();
		yield return null;
	}
}

