using UnityEngine;
using System.Collections;

public class StatusEffectContainer : MonoBehaviour
{
	public static StatusEffectContainer instance;

	[System.Serializable]
	public class EnemyStatusDictionaryEntry
	{
		public string name;
		public GameObject status;
	}

	public EnemyStatusDictionaryEntry[] statuses;

	void Awake()
	{
		// make this a singleton
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (this.gameObject);
	}

	public GameObject GetStatus(string name)
	{
		foreach (EnemyStatusDictionaryEntry entry in statuses)
		{
			if (entry.name.Equals (name))
				return entry.status;
		}
		throw new UnityEngine.Assertions.AssertionException (
			"StatusEffectContainer.cs:",
			"Cannot find EnemyStatus with name " + name
		);
	}
}

