using UnityEngine;
using System.Collections.Generic;

public class HealthMeter : MonoBehaviour {

	public List<HealthIndicator> healthIndicators = new List<HealthIndicator>();
	public GameObject healthIndicatorPrefab;

	private Player player;
	private bool initialized;


	private void Awake()
	{
		player = GetComponentInParent<GUIManager>().player;
	}

	void OnEnable()
	{
		player.OnPlayerInitialized += Init;
	}

	public void OnDisable()
	{
		player.OnPlayerInitialized -= Init;
	}

	public void Init () 
	{
		initialized = true;
		foreach (HealthIndicator o in healthIndicators) {
			o.gameObject.SetActive(false);
		}
		for(int i = 0; i < player.hero.numHearts; i ++)
		{
			if (i < healthIndicators.Count) {
				healthIndicators[i].gameObject.SetActive(true);
			}
			else {
				GameObject obj = Instantiate (healthIndicatorPrefab);
				obj.transform.SetParent (this.transform, false);
				healthIndicators.Add(obj.GetComponent<HealthIndicator> ());
			}
			healthIndicators[i].transform.SetSiblingIndex(i);
		}
	}

	void Update() {
		if (!initialized)
			return;
		int numFullSoftHearts = 0;
		int numFullHardHearts = 0;
		float softFrac = 0;
		float hardFrac = 0;
		GetHeartInfo(player.softHealth, out numFullSoftHearts, out softFrac);
		GetHeartInfo(player.hardHealth, out numFullHardHearts, out hardFrac);
		for (int i = 0; i < player.hero.numHearts; i ++) {
			// Soft Health
			if (i < numFullSoftHearts)
				healthIndicators[i].SetSoftHealth(1);
			else if (i == numFullSoftHearts)
				healthIndicators[i].SetSoftHealth(softFrac);
			else
				healthIndicators[i].SetSoftHealth(0);
			// Hard Health
			if (i < numFullHardHearts)
				healthIndicators[i].SetHardHealth(1);
			else if (i == numFullHardHearts)
				healthIndicators[i].SetHardHealth(hardFrac);
			else
				healthIndicators[i].SetHardHealth(0);
		}
	}

	private void GetHeartInfo(float val, out int numFullHearts, out float frac) {
		numFullHearts = (int)val / Player.HEALTH_PER_HEART;
		frac = (val % Player.HEALTH_PER_HEART) / Player.HEALTH_PER_HEART;
	}
}
