using UnityEngine;
using System.Collections;

public class TwinsAbility : EnemyAbility
{
	//public GameObject twinsEffectAbilityPrefab;
	private EnemyManager enemyManager;
	private GameObject twinPrefab;

	public override void Init (Enemy enemy)
	{
		base.Init (enemy);
		enemyManager = transform.GetComponentInParent<EnemyManager> ();
		enemy.OnEnemyInit += OnInit;
	}

	private void OnInit()
	{
		enemy.invincible = true;
		Invoke ("Split", 1f);
	}

	private void Split()
	{
		enemy.invincible = false;
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

