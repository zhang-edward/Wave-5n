using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAbilitiesManager : MonoBehaviour
{
	public static EnemyAbilitiesManager instance;

	[System.Serializable]
	public class EnemyAbilityDictionaryEntry
	{
		public string name;
		public GameObject ability;
	}
	public List<EnemyAbilityDictionaryEntry> enemyAbilities;

	void Awake()
	{
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);
	}


	public GameObject GetAbility(string abilityName)
	{
		foreach (EnemyAbilityDictionaryEntry entry in enemyAbilities)
		{
			if (entry.name.Equals(abilityName))
			{
				return entry.ability;
			}
		}
		Debug.LogError ("Cannot find ability with name " + abilityName);
		return null;
	}
}

