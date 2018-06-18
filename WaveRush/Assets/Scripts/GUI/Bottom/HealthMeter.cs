using UnityEngine;
using System.Collections;

public class HealthMeter : MonoBehaviour {

	private Player player;
	public HealthIndicator[] healthIndicators;
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
		healthIndicators = new HealthIndicator[player.hero.numHearts];
		for(int i = 0; i < player.hero.numHearts; i ++)
		{
			GameObject obj = Instantiate (healthIndicatorPrefab);
			obj.transform.SetParent (this.transform, false);
			healthIndicators [i] = obj.GetComponent<HealthIndicator> ();
		}
	}

	public void UpdateHealthMeter(int amt)
	{
		// int healthPerHeart = Player.HEALTH_PER_QUARTER_HEART * 4;
		// // Set the remaining fraction of a heart
		// if (player.hardHealth % healthPerHeart != 0) {
		// 	healthIndicators[player.hardHealth / healthPerHeart].SetQuarters ((player.hardHealth % healthPerHeart) / Player.HEALTH_PER_QUARTER_HEART);
		// }
	}

	void Update() {
		// int i = 0;
		int healthPerHeart = Player.HEALTH_PER_QUARTER_HEART * 4;
		// int numFullHearts = player.softHealth / healthPerHeart;
		// // Set full hearts to full
		// while (i < numFullHearts) {
		// 	healthIndicators[i].SetFull();
		// 	i ++;
		// }
		// // Skip the "fraction" heart
		// i ++;
		// // Set empty hearts to empty
		// while (i < healthIndicators.Length) {
		// 	healthIndicators[i].SetEmpty();
		// 	i ++;
		// }
		// // Set the fraction heart
		// if (player.softHealth < player.maxHealth) {
		int numFullSoftHearts = 0;
		int numFullHardHearts = 0;
		float softFrac = 0;
		float hardFrac = 0;
		GetHeartInfo(player.softHealth, out numFullSoftHearts, out softFrac);
		GetHeartInfo(player.hardHealth, out numFullHardHearts, out hardFrac);
		for (int i = 0; i < healthIndicators.Length; i ++) {
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

	public void OnDisable()
	{
		player.OnPlayerDamaged -= UpdateHealthMeter;
		player.OnPlayerHealed -= UpdateHealthMeter;
		player.OnPlayerInitialized -= Init;
	}
}
