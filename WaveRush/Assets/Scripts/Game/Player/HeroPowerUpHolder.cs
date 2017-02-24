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
				return powerUpPrefab.GetComponent<HeroPowerUp> ().info.powerUpName;
			}
		}
		public GameObject powerUpPrefab;
	}

	public List<HeroPowerUpDictionaryEntry> powerUpPrefabs;			// dictionary database of all available power ups
	[HideInInspector]
	public List<HeroPowerUp> activePowerUps;						// list of powerups that are active on the player in the game

	public delegate void OnPowerUpsChanged();
	public OnPowerUpsChanged OnPowerUpAdded;

	void Awake()
	{
		hero = GetComponent<PlayerHero> ();
	}

	public void Init()
	{
		// get universal player powerups (apply status effect)
		foreach (HeroPowerUpDictionaryEntry entry in hero.player.powerUpPrefabs)
		{
			powerUpPrefabs.Add (entry);
		}
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
			if (powerUp.info.powerUpName.Equals (activePowerUp.info.powerUpName))
				return activePowerUp;
		}
		return null;
	}
}

