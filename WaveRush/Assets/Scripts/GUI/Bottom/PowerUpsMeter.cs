using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsMeter : MonoBehaviour {

	public Player player;
	private HeroPowerUpHolder powerUpHolder;

	public List<UIPowerUpIcon> powerUpIcons;
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
		while (powerUpIcons.Count < powerUpHolder.activePowerUps.Count)
		{
			GameObject o = Instantiate (powerUpIconPrefab);
			o.transform.SetParent (transform, false);
			powerUpIcons.Add (o.GetComponent<UIPowerUpIcon> ());
		}
		// update the icon for each icon
		for (int i = 0; i < powerUpHolder.activePowerUps.Count; i ++)
		{
			powerUpIcons [i].Init (powerUpHolder.activePowerUps [i]);
		}
	}

	void OnDisable()
	{
		player.OnPlayerInitialized -= Init;
		player.hero.powerUpHolder.OnPowerUpAdded -= UpdatePowerUpsMeter;
	}
}
