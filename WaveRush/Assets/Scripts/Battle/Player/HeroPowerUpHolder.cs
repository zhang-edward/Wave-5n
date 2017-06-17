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
				return powerUpPrefab.GetComponent<HeroPowerUp> ().data.powerUpName;
			}
		}
		public GameObject powerUpPrefab;
		public HeroPowerUpDictionaryEntry(GameObject prefab)
		{
			powerUpPrefab = prefab;
		}
	}

	//public List<HeroPowerUpDictionaryEntry> powerUpPrefabs { get; private set; }			// dictionary database of all available power ups
	public List<HeroPowerUp> powerUps;                  // list of powerups that are active on the player in the game
	public int numActivePowerUps;

	public delegate void OnPowerUpsChanged();
	public OnPowerUpsChanged OnPowerUpAdded;


	public void Init()
	{
		hero = GetComponent<PlayerHero>();

		//powerUpPrefabs = new List<HeroPowerUpDictionaryEntry>();
		HeroPowerUpListData powerUpListData = DataManager.GetPowerUpListData(hero.heroType);
		int level = 0;
		//int numPowerUpsUnlocked = GameManager.instance.saveGame.GetHeroData(hero.heroType).numPowerUpsUnlocked;
		for (int i = 0; i < level; i ++)
		{
			HeroPowerUp powerUp = powerUpListData.powerUps[i];						// get the powerUp prefab data
			//powerUpPrefabs.Add(new HeroPowerUpDictionaryEntry(powerUp.gameObject));
			GameObject o = Instantiate(powerUp.gameObject);                         // instantiate the prefab
			powerUps.Add(o.GetComponent<HeroPowerUp>());
			o.transform.SetParent(transform);
			o.transform.localPosition = Vector3.zero;
			o.SetActive(false);
		}
	}

	public HeroPowerUp GetPowerUp(string name)
	{
		foreach (HeroPowerUp powerUp in powerUps)
		{
			if (powerUp.data.powerUpName.Equals(name))
				return powerUp;
		}
		/*foreach (HeroPowerUpDictionaryEntry entry in powerUpPrefabs)
		{
			if (entry.name.Equals (name))
				return entry.powerUpPrefab;
		}*/
		throw new UnityEngine.Assertions.AssertionException ("HeroPowerUpHolder.cs",
			"Cannot find HeroPowerUp with name" + "\"" + name + "\"");
	}

	public void AddPowerUp(string name)
	{
		// GameObject prefab = GetPowerUp (name);
		HeroPowerUp selectedPowerUp = GetPowerUp(name).GetComponent<HeroPowerUp> ();
		// test if this hero already has the selected power up
		// HeroPowerUp existingPowerUp = GetPowerUp (prefabPowerUp);
		// if (existingPowerUp != null)
		if (selectedPowerUp.isActive)
		{
			// if yes, then stack the powerup
			selectedPowerUp.Stack ();
		}
		else
		{
			selectedPowerUp.gameObject.SetActive(true);
			selectedPowerUp.Activate(hero);
			numActivePowerUps++;
		}
		if (OnPowerUpAdded != null)
			OnPowerUpAdded();
		// GameObject o = Instantiate (prefab);
		// o.transform.SetParent (transform);
		// o.transform.localPosition = Vector3.zero;
		// HeroPowerUp powerUp = o.GetComponent<HeroPowerUp> ();
		// powerUps.Add (selectedPowerUp);

	}

	private HeroPowerUp GetPowerUp(HeroPowerUp powerUp)
	{
		foreach (HeroPowerUp activePowerUp in powerUps)
		{
			if (powerUp.data.powerUpName.Equals (activePowerUp.data.powerUpName))
				return activePowerUp;
		}
		return null;
	}
}

