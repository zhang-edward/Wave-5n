﻿using UnityEngine;
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
		twinPrefab = enemy.transform.parent.gameObject;
		twinPrefab.GetComponentInChildren<Enemy> ().abilities.Clear();
		twinPrefab.GetComponentInChildren<Enemy> ().AddAbility (
			EnemyAbilitiesManager.instance.GetAbility ("TwinsEffect"));
		for (int i = 0; i < 2; i ++)
		{
			enemyManager.SpawnEnemy (twinPrefab, 
				UtilMethods.RandomOffsetVector2(transform.position, 1.0f));
		}
		enemy.transform.parent.gameObject.SetActive(false);
	}
}

