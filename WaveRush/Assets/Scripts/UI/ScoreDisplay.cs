using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreDisplay : MonoBehaviour {

	public Text enemiesDefeated;
	public Text wavesSurvived;

	public void DisplayScores(HeroType type)
	{
		string hero = type.ToString();
		ScoreManager sm = GameManager.instance.scoreManager;
		if (sm.highScores.ContainsKey(hero))
		{
			ScoreManager.Score score = sm.highScores [hero];
			enemiesDefeated.text = score.enemiesDefeated.ToString();
			wavesSurvived.text = score.wavesSurvived.ToString();	
		}
		else
		{
			enemiesDefeated.text = "0";
			wavesSurvived.text = "0";	
		}
	}
}
