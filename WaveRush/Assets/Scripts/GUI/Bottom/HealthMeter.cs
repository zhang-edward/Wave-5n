using UnityEngine;
using System.Collections;

public class HealthMeter : MonoBehaviour {

	private Player player;
	public UIHeart[] healthIndicators;
	public GameObject healthIndicatorPrefab;

	private void Awake()
	{
		player = GetComponentInParent<GUIManager>().player;
	}

	void OnEnable()
	{
		player.OnPlayerDamaged += UpdateHealthMeter;
		player.OnPlayerHealed += UpdateHealthMeter;
		player.OnPlayerInitialized += Init;
	}

	public void Init () 
	{
		healthIndicators = new UIHeart[player.maxHealth / 4];
		for(int i = 0; i < player.maxHealth; i += 4)
		{
			GameObject obj = Instantiate (healthIndicatorPrefab);
			obj.transform.SetParent (this.transform, false);
			healthIndicators [i / 4] = obj.GetComponent<UIHeart> ();
		}
	}

	public void UpdateHealthMeter(int amt)
	{
		foreach (UIHeart indicator in healthIndicators)
			indicator.SetEmpty();
		int i = 0;
		while (i < player.health)
		{
			if (i % 4 == 0)
			{
				healthIndicators [i / 4].SetFull ();
			}
			i++;
		}
		if (i % 4 != 0)
		{
			healthIndicators [i / 4].SetQuarters (i % 4);
		}
	}

	public void OnDisable()
	{
		player.OnPlayerDamaged -= UpdateHealthMeter;
		player.OnPlayerHealed -= UpdateHealthMeter;
		player.OnPlayerInitialized -= Init;
	}
}
