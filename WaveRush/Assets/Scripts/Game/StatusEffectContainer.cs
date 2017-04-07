using UnityEngine;
using System.Collections;

public class StatusEffectContainer : MonoBehaviour
{
	public static StatusEffectContainer instance;

	public GameObject[] statuses;

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
		foreach (GameObject obj in statuses)
		{
			if (obj.GetComponent<EnemyStatus>().statusName.Equals (name))
				return obj;
		}
		throw new UnityEngine.Assertions.AssertionException (
			"StatusEffectContainer.cs:",
			"Cannot find EnemyStatus with name " + name
		);
	}
}

