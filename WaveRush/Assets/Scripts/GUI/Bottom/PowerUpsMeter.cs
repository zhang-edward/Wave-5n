using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsMeter : MonoBehaviour {

	public Player player;
	private HeroPowerUpHolder powerUpHolder;

	public List<PowerUpIcon> powerUpIcons = new List<PowerUpIcon>();
	public GameObject powerUpIconPrefab;

	void OnEnable()
	{
		player.OnPlayerInitialized += Init;
	}

	private void Init()
	{
		powerUpHolder = player.hero.powerUpHolder;
		UpdatePowerUpsMeter ();
		player.hero.powerUpHolder.OnPowerUpAdded += UpdatePowerUpsMeter;
	}

	private void UpdatePowerUpsMeter()
	{
		// instantiate an icon for each power up that the player has
		while (powerUpIcons.Count < powerUpHolder.numActivePowerUps)
		{
			GameObject o = Instantiate (powerUpIconPrefab);
			o.transform.SetParent (transform, false);
			powerUpIcons.Add (o.GetComponent<PowerUpIcon> ());
		}
		// update the icon for each icon
		for (int i = 0; i < powerUpHolder.numActivePowerUps; i ++)
		{
			//print("Updating index:" + i);
			powerUpIcons [i].Init (powerUpHolder.powerUps [i]);
		}
	}

	void OnDisable()
	{
		player.OnPlayerInitialized -= Init;
		player.hero.powerUpHolder.OnPowerUpAdded -= UpdatePowerUpsMeter;
	}
}
