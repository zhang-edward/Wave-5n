using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpsMeter : MonoBehaviour {

	private Player player;
	private HeroPowerUpManager powerUpHolder;

	public List<PowerUpIcon> powerUpIcons = new List<PowerUpIcon>();
	public GameObject powerUpIconPrefab;

	private void Awake()
	{
		player = GetComponentInParent<GUIManager>().player;
	}

	void OnEnable()
	{
		player.OnPlayerInitialized += Init;
	}

	private void Init()
	{
		powerUpHolder = player.hero.powerUpManager;
		UpdatePowerUpsMeter ();
		//player.hero.powerUpManager.OnPowerUpAdded += UpdatePowerUpsMeter;
	}

	private void UpdatePowerUpsMeter()
	{
		foreach (PowerUpIcon o in powerUpIcons) {
			o.gameObject.SetActive(false);
		}
		// // instantiate an icon for each power up that the player has
		// for (int i = 0; i < powerUpHolder.numActivePowerUps; i ++) {
		// 	if (powerUpIcons[i] == null) {

		// 	}
		// }
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
			powerUpIcons[i].Init (powerUpHolder.powerUps[i]);
			powerUpIcons[i].gameObject.SetActive(true);
		}
	}

	void OnDisable()
	{
		player.OnPlayerInitialized -= Init;
		player.hero.powerUpManager.OnPowerUpAdded -= UpdatePowerUpsMeter;
	}
}
