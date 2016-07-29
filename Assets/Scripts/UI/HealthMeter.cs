using UnityEngine;
using System.Collections;

namespace UI
{
	public class HealthMeter : MonoBehaviour {

		public Player player;
		public UIHeart[] healthIndicators;
		public GameObject healthIndicatorPrefab;

		void OnEnabled()
		{
			player.OnPlayerDamaged += UpdateHealthMeter;
			player.OnPlayerHealed += UpdateHealthMeter;
			player.OnPlayerInitialized += Init;
		}

		public void Init () 
		{
			healthIndicators = new UIHeart[player.maxHealth];
			Debug.Log (player.maxHealth);
			for(int i = 0; i < player.maxHealth; i ++)
			{
				GameObject obj = Instantiate (healthIndicatorPrefab);
				obj.transform.SetParent (this.transform, false);
				healthIndicators [i] = obj.GetComponent<UIHeart> ();
			}
		}

		public void UpdateHealthMeter(int amt)
		{
			foreach (UIHeart indicator in healthIndicators)
				indicator.SetEmpty();
			for (int i = 0; i < player.health; i ++)
			{
				healthIndicators [i].SetFull ();
			}
		}

		public void OnDisable()
		{
			player.OnPlayerDamaged -= UpdateHealthMeter;
			player.OnPlayerHealed -= UpdateHealthMeter;
			player.OnPlayerInitialized -= Init;
		}
	}
}