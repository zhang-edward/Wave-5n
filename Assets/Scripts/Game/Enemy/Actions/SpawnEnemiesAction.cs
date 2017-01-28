using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnEnemiesAction : EnemyAction
{
	private Animator anim;
	private EnemyManager enemyManager;
	private List<GameObject> minions = new List<GameObject>();

	[Header("Properties")]
	public float chargeTime;
	public float spawnTime;
	public int numToSpawn;
	public float spawnOffset = 4.0f;
	public int maxMinions;
	public GameObject[] minionPrefabs;
	[Header("AnimationStates")]
	public string chargeState;
	public string actionState;
	[Header("Audio")]
	public AudioClip spawnSound;

	public event OnActionStateChanged onSpawn;

	public override void Init (Enemy e, OnActionStateChanged onActionFinished)
	{
		base.Init (e, onActionFinished);
		anim = e.anim;
		enemyManager = GetComponentInParent<EnemyManager> ();
	}

	public override void Execute ()
	{
		StartCoroutine (UpdateState ());
	}

	public override bool CanExecute ()
	{
		CleanMinionsList ();
		if (minions.Count >= maxMinions)
			return false;
		else
			return base.CanExecute ();
	}

	public override void Interrupt ()
	{
		base.Interrupt ();
		StopAllCoroutines ();
	}

	private IEnumerator UpdateState()
	{
		anim.CrossFade (chargeState, 0);
		yield return new WaitForSeconds (chargeTime);

		anim.CrossFade (actionState, 0);
		Spawn ();

		if (onSpawn != null)
			onSpawn ();
		
		yield return new WaitForSeconds (spawnTime);

		onActionFinished ();
	}

	// Spawn minions
	private void Spawn()
	{
		SoundManager.instance.RandomizeSFX (spawnSound);
		// Spawn 4 random enemies
		for(int i = 0; i < numToSpawn; i ++)
		{
			if (minions.Count > maxMinions)
				break;
			// place spawned enemies within a radius of 4 from this boss entity
			GameObject o = enemyManager.SpawnEnemy (minionPrefabs [Random.Range (0, minionPrefabs.Length)],
				UtilMethods.RandomOffsetVector2(transform.position, spawnOffset));
			minions.Add (o);
		}
	}

	private void CleanMinionsList()
	{
		for (int i = minions.Count - 1; i >= 0; i --)
		{
			GameObject o = minions [i];
			if (o == null || !o.activeInHierarchy)
				minions.Remove (o);
		}
	}
}

