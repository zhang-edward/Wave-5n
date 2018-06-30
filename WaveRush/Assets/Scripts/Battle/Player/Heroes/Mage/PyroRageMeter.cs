using UnityEngine;
using UnityEngine.UI;

public class PyroRageMeter : MonoBehaviour {

	public Image[] fillImages;

	private Pyro_Rage pyroRage;
	
	public void Init(Pyro_Rage pyroRage) {
		this.pyroRage = pyroRage;
	}

	void Update() {
		float colorCycleFreq;
		if (pyroRage.maxRageTimer > 0)
			colorCycleFreq = 2.0f;
		else
			colorCycleFreq = 0.5f;
		foreach (Image fill in fillImages) {
			fill.fillAmount = pyroRage.rage;
			fill.color = Color.Lerp(Color.red, Color.yellow, Mathf.PingPong(Time.time * colorCycleFreq, 1.0f));	// Fiery color effect
		}
	}
}