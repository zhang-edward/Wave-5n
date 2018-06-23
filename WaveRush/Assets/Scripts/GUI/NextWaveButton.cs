using UnityEngine;
using UnityEngine.UI;

public class NextWaveButton : MonoBehaviour {
	
	public EnemyManager enemyManager;
	public Image countdownImage;
	public TMPro.TMP_Text text;

	void Update() {
		countdownImage.fillAmount = enemyManager.timeLeftBeforeNextWave / EnemyManager.WAVE_SPAWN_DELAY;
		text.text = "Next Wave in: " + ((int)enemyManager.timeLeftBeforeNextWave).ToString();
	}
}