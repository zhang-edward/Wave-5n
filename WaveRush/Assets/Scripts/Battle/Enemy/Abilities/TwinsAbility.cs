using UnityEngine;
using System.Collections;

public class TwinsAbility : EnemyAbility
{
	public SimpleAnimationPlayer anim;
	private EnemyManager enemyManager;
	private GameObject twinPrefab;		// mini-fied version of this prefab to spawn


	public override void Init (Enemy enemy)
	{
		base.Init (enemy);
		enemyManager = transform.GetComponentInParent<EnemyManager> ();
		enemy.OnEnemyDamaged += OnEnemyDamaged;
		StartCoroutine ("CheckIfPlayerInRange");
	}

	public void OnEnemyDamaged(int amt)
	{
		StopAllCoroutines ();
		StartCoroutine ("SplitEffect");
	}

	private IEnumerator CheckIfPlayerInRange()
	{
		bool hasSplit = false;
		while (!hasSplit)
		{
			if (PlayerInRange ())
			{
				StopAllCoroutines ();
				StartCoroutine ("SplitEffect");
				hasSplit = true;
			}
			yield return new WaitForSeconds (1.0f);
		}
	}

	protected virtual bool PlayerInRange()
	{
		Collider2D[] cols = Physics2D.OverlapCircleAll (transform.position, 2f);
		foreach (Collider2D col in cols)
		{
			if (col.CompareTag ("Player"))
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator SplitEffect()
	{
		enemy.invincible = true;
		enemy.ForceDisable (1.0f);
		anim.Play ();
		anim.transform.localScale = Vector3.one * enemy.statusIconSize;
		while (anim.isPlaying)
			yield return null;
		enemy.invincible = false;
		Split ();
	}

	private void Split()
	{
		twinPrefab = enemy.transform.parent.gameObject;
		twinPrefab.GetComponentInChildren<Enemy> ().abilities.Clear();
		twinPrefab.GetComponentInChildren<Enemy> ().AddAbility (
			EnemyAbilitiesManager.instance.GetAbility ("Dwarf"));
		for (int i = 0; i < 2; i ++)
		{
			enemyManager.SpawnEnemyForcePosition (twinPrefab, transform.position);
		}
		enemy.transform.parent.gameObject.SetActive(false);
	}
}

