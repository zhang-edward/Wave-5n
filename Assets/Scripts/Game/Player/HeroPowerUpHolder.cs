using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HeroPowerUpHolder : MonoBehaviour
{
	private PlayerHero hero;
	[System.Serializable]
	public class HeroPowerUpDictionaryEntry 
	{
		public string name { 
			get {
				return powerUpPrefab.GetComponent<HeroPowerUp> ().powerUpName;
			}
		}
		public GameObject powerUpPrefab;
	}
	public HeroPowerUpDictionaryEntry[] powerUpPrefabs;
	public List<HeroPowerUp> activePowerUps;

	public delegate void OnPowerUpsChanged();
	public OnPowerUpsChanged OnPowerUpAdded;

	void Awake()
	{
		hero = GetComponent<PlayerHero> ();
	}

	public GameObject GetPowerUp(string name)
	{
		foreach (HeroPowerUpDictionaryEntry entry in powerUpPrefabs)
		{
			if (entry.name.Equals (name))
				return entry.powerUpPrefab;
		}
		throw new UnityEngine.Assertions.AssertionException ("HeroPowerUpHolder.cs",
			"Cannot find HeroPowerUp with name" + "\"" + name + "\"");
	}

	public void AddPowerUp(string name)
	{
		// test if this hero has the selected power up
		GameObject prefab = GetPowerUp (name);
		HeroPowerUp prefabPowerUp = prefab.GetComponent<HeroPowerUp> ();
		HeroPowerUp existingPowerUp = GetPowerUp (prefabPowerUp);
		if (existingPowerUp != null)
		{
			existingPowerUp.Stack ();
			return;
		}

		GameObject o = Instantiate (prefab);
		o.transform.SetParent (transform);
		o.transform.localPosition = Vector3.zero;
		HeroPowerUp powerUp = o.GetComponent<HeroPowerUp> ();
		activePowerUps.Add (powerUp);
		powerUp.Activate (hero);

		if (OnPowerUpAdded != null)
			OnPowerUpAdded ();
	}

	private HeroPowerUp GetPowerUp(HeroPowerUp powerUp)
	{
		foreach (HeroPowerUp activePowerUp in activePowerUps)
		{
			if (powerUp.powerUpName.Equals (activePowerUp.powerUpName))
				return activePowerUp;
		}
		return null;
	}
}

