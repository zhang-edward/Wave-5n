using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	public Text enemiesDefeated;
	public Text wavesSurvived;
	public Text maxCombo;

	public void DisplayScores(HeroType type)
	{
		ScoreManager sm = GameManager.instance.scoreManager;
		if (sm.highScores.ContainsKey(type))
		{
			ScoreManager.Score score = sm.highScores [type];
			enemiesDefeated.text = score.enemiesDefeated.ToString();
			wavesSurvived.text = score.wavesSurvived.ToString();
			maxCombo.text = score.maxCombo.ToString();
		}
		else
		{
			enemiesDefeated.text = "-";
			wavesSurvived.text = "-";
			maxCombo.text = "-";
		}
	}
}
